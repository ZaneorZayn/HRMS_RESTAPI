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
        var existingAttendance = await _context.Attendances.
            FirstOrDefaultAsync(a => a.SystemUserId == systemUserId && a.Date == today);

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
            string emailBody = $"Dear {employee.Name},\n\nYou checked in late today at {attendance.ClockIn}. Please be on time.";

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
     
     
     var existingAttendance = await _context.Attendances.
         FirstOrDefaultAsync(a => a.SystemUserId == systemUserId && a.Date == today);

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

    public async Task<List<Attendance>> GetAttendancesByToday()
    {
       var today = DateTime.Now.Date;
       
       return await _context.Attendances.
           Where(a => a.Date == today).
           Include(a => a.Employee).
           //Include(a => a.SystemUser).
           ToListAsync();
    }
}

    
