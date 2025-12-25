using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Dto.AttendanceDto;
using hrms_api.Enum;
using hrms_api.Model;
using hrms_api.Repository.EmailRepository;
using Microsoft.EntityFrameworkCore;

namespace hrms_api.Repository.AttendanceRepository;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public AttendanceRepository(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task MarkCheckInAsync(int systemUserId)
    {
        var today = DateTime.Now.Date;
        var currentTime = DateTime.Now.TimeOfDay;
        var workStartTime = new TimeSpan(8, 0, 0);
        var existingAttendance =
            await _context.Attendances.FirstOrDefaultAsync(a => a.SystemUserId == systemUserId && a.Date == today);

        if (existingAttendance != null)
        {
            throw new Exception("Attendance already exists");
        }

        // Fetch the employee linked to the systemUserId
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.SystemUserId == systemUserId);
        if (employee == null)
        {
            throw new Exception("Employee not found for the provided SystemUserId.");
        }

        var attendance = new Attendance
        {
            SystemUserId = systemUserId,
            EmployeeId = employee.Id,
            Date = today,
            ClockIn = currentTime,
            Status = currentTime > workStartTime ? AttendanceStatus.Late : AttendanceStatus.Present
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();

        if (attendance.Status == AttendanceStatus.Late)
        {
            string emailSubject = "Late Check-In Notification";
            string emailBody =
                $"Dear {employee.Name},\n\nYou checked in late today at {attendance.ClockIn}. Please be on time.";

            var emailRequest = new EmailDto
            {
                ToEmail = employee.Email!,
                Subject = emailSubject,
                Body = emailBody
            };

            await _emailService.SendEmailAsync(emailRequest);
        }
    }

    public async Task MarkCheckOutAsync(int systemUserId)
    {
        var today = DateTime.Now.Date;
        var currentTime = DateTime.Now.TimeOfDay;
        var workStartTime = new TimeSpan(8, 0, 0);
        var workEndTime = new TimeSpan(17, 0, 0);
        var regularWorkTime = workEndTime - workStartTime;


        var existingAttendance =
            await _context.Attendances.FirstOrDefaultAsync(a => a.SystemUserId == systemUserId && a.Date == today);

        if (existingAttendance == null && existingAttendance.ClockOut != null)
        {
            throw new Exception("Please check-in fist");
        }

        existingAttendance.ClockOut = currentTime;
        existingAttendance.TotalHours = existingAttendance.ClockOut.Value - existingAttendance.ClockIn;

        if (existingAttendance.TotalHours > workEndTime)
        {
            existingAttendance.OverTime = existingAttendance.ClockOut.Value - workEndTime;
        }

        if (existingAttendance.Status == AttendanceStatus.Late)
        {
            existingAttendance.Status = AttendanceStatus.Late;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<AttendanceDto>> GetAttendancesByToday()
    {
        var today = DateTime.Now.Date;

        var employees = await _context.Employees.ToListAsync();
        var attendances = await _context.Attendances
            .Where(a => a.Date == today)
            .ToListAsync();

        var result = employees.Select(emp =>
        {
            var attendance = attendances.FirstOrDefault(a => a.EmployeeId == emp.Id);

            if (attendance == null)
            {
                return new AttendanceDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    Date = today,
                    Status = AttendanceStatus.Absent.ToString(),
                    ClockIn = TimeSpan.Zero,
                    ClockOut = TimeSpan.Zero
                };
            }

            return new AttendanceDto
            {
                EmployeeId = emp.Id,
                EmployeeName = emp.Name,
                Date = attendance.Date,
                Status = attendance.Status.ToString(),
                ClockIn = attendance.ClockIn,
                ClockOut = attendance.ClockOut
            };
        }).ToList();

        return result;
    }


    public async Task<string> ScanAttendanceAsync(AttendanceScanDto attendanceScanDto, int systemUserId)
    {
        if (string.IsNullOrWhiteSpace(attendanceScanDto.QrContent))
            throw new Exception("QR content is missing.");

        // Example content: HRMS|LAT:11.56|LNG:104.91
        var parts = attendanceScanDto.QrContent.Split("|", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3 || !parts[1].StartsWith("LAT:") || !parts[2].StartsWith("LONG:"))
            throw new Exception("Invalid QR code content format.");

        if (!double.TryParse(parts[1].Replace("LAT:", ""), out double officeLat) ||
            !double.TryParse(parts[2].Replace("LONG:", ""), out double officeLng))
            throw new Exception("QR code contains invalid coordinates.");

        double distance =
            GetDistanceInMeters(attendanceScanDto.Latitude, attendanceScanDto.Longitude, officeLat, officeLng);
        if (distance > 100)
            throw new Exception($"You are too far from office to mark attendance. Distance: {distance:F2} meters");

        var today = DateTime.Now.Date;
        var now = DateTime.Now.TimeOfDay;
        var workStartTime = new TimeSpan(8, 0, 0);
        var workEndTime = new TimeSpan(17, 0, 0);

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.SystemUserId == systemUserId)
                       ?? throw new Exception("Employee not found.");

        var attendance = await _context.Attendances
            .FirstOrDefaultAsync(a => a.SystemUserId == systemUserId && a.Date == today);

        if (attendance == null)
        {
            // Check-in
            var newAttendance = new Attendance
            {
                SystemUserId = systemUserId,
                EmployeeId = employee.Id,
                Date = today,
                ClockIn = now,
                Status = now > workStartTime ? AttendanceStatus.Late : AttendanceStatus.Present
            };

            _context.Attendances.Add(newAttendance);
            await _context.SaveChangesAsync();
            return " Check-in successful.";
        }

        if (attendance.ClockOut == null)
        {
            // Check-out
            attendance.ClockOut = now;
            attendance.TotalHours = attendance.ClockOut.Value - attendance.ClockIn;

            if (attendance.TotalHours > (workEndTime - workStartTime))
                attendance.OverTime = attendance.TotalHours - (workEndTime - workStartTime);

            await _context.SaveChangesAsync();
            return "Check-out successful.";
        }
        else
        {
            return "ℹ You have already checked out for today.";
        }
    }

    public async Task<List<Attendance>> GetAttendanceUser(int systemUserId)
    {
        return await _context.Attendances.Where(a => a.SystemUserId == systemUserId).ToListAsync();
    }

    private double GetDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
{
    const double EarthRadius = 6371e3; // in meters

    var lat1Rad = lat1 * Math.PI / 180;
    var lat2Rad = lat2 * Math.PI / 180;
    var deltaLat = (lat2 - lat1) * Math.PI / 180;
    var deltaLon = (lon2 - lon1) * Math.PI / 180;

    var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
            Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
            Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

    return EarthRadius * c;
}

}