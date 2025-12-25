using hrms_api.Data;
using hrms_api.Dto.LeaveRequest;
using hrms_api.Enum;
using hrms_api.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms_api.Repository.LeaveRequestRepository;

public class LeaverRequestRepository : ILeaveRequestRepository
{
    private readonly AppDbContext _context;

    public LeaverRequestRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<LeaveRequest?> GetByIdAsync(int id)
    {
          var leaveRequest =  await _context.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.ApprovedBy)
            .FirstOrDefaultAsync(l => l.Id == id);

          if (leaveRequest == null)
          {
              throw new Exception("LeaveRequest not found");
          }

          return leaveRequest;
    }

    public async Task<List<LeaveRequestResponseDto>> GetAllAsync(LeaveRequestFilter filter)
    {
        var query = _context.LeaveRequests
            .Include(l => l.Employee)    // include employee info (name, avatar, etc.)
            .Include(l => l.ApprovedBy)
            .Include(l => l.RejectedBy)
            .AsQueryable();

        // Filter by status if provided
        if (filter.Status.HasValue)
            query = query.Where(l => l.LeaveStatus == filter.Status.Value);

        // Sorting
        var sortOrder = string.IsNullOrWhiteSpace(filter.SortOrder) ? "desc" : filter.SortOrder.ToLower();
        query = sortOrder == "asc"
            ? query.OrderBy(l => l.RequestDate)
            : query.OrderByDescending(l => l.RequestDate);

        // Pagination + projection
        var result = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(l => new LeaveRequestResponseDto()
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.Name,
                EmployeeAvatar = l.Employee.ImageUrl, // ✅ include avatar if needed
                LeaveType = l.LeaveType,
                LeaveSession = l.LeaveSession,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.LeaveStatus,
                RequestedDate = l.RequestDate,
                ApprovedAtDate = l.ApprovedDate,
                ApprovedByName = l.ApprovedBy != null ? l.ApprovedBy.Name : null,
                RejectedAtDate = l.RejectedDate,
                RejectedByName = l.RejectedBy != null ? l.RejectedBy.Name : null,
                RejectionReason = l.RejectionReason
            })
            .ToListAsync();

        return result;
    }


    public Task<int> CountAsync(LeaveRequestFilter filter)
    {
        throw new NotImplementedException();
    }

    public async Task<LeaveRequest> CreateAsync(LeaveRequestDto leave, int systemUserId)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.SystemUserId == systemUserId);
        if (employee == null)
            throw new Exception($"No employee linked to SystemUserId {systemUserId}");

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = employee.Id,
            LeaveType = leave.LeaveType,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            LeaveSession = leave.LeaveSession,
            LeaveStatus = LeaveStatus.Pending,
            RequestDate = DateTime.UtcNow,
            ApprovedById = null
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task<LeaveRequestResponseDto> ApproveAsync(int leaveRequestId, int managerId)
{
    var leave = await _context.LeaveRequests
        .Include(l => l.Employee)
        .ThenInclude(e => e.Department)
        .FirstOrDefaultAsync(l => l.Id == leaveRequestId);

    if (leave == null)
        throw new Exception("Leave request not found");

    // Make sure the manager exists and has a department
    var manager = await _context.Employees
        .Include(m => m.Department)
        .FirstOrDefaultAsync(m => m.Id == managerId);

    if (manager == null)
        throw new Exception("Manager not found");

    if (manager.DepartmentId == null || leave.Employee?.DepartmentId == null)
        throw new Exception("Either manager or employee has no department assigned");

    // Only allow if manager is the head of employee's department
    if (leave.Employee.DepartmentId != manager.DepartmentId || manager.Id != leave.Employee.Department?.ManagerId)
        throw new UnauthorizedAccessException("Only the department manager can approve this leave request.");

    // Approve leave
    leave.LeaveStatus = LeaveStatus.Approved;
    leave.ApprovedDate = DateTime.UtcNow;
    leave.ApprovedById = managerId;

    // Update attendance (assuming Employee has SystemUserId)
    var systemUserId = leave.Employee.SystemUserId ?? throw new Exception("Employee has no linked SystemUser");

    for (var date = leave.StartDate.Date; date <= leave.EndDate.Date; date = date.AddDays(1))
    {
        var attendance = await _context.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == leave.EmployeeId && a.Date == date);

        if (attendance == null)
        {
            _context.Attendances.Add(new Attendance
            {
                EmployeeId = leave.EmployeeId,
                SystemUserId = systemUserId,
                Date = date,
                Status = AttendanceStatus.OnLeave
            });
        }
        else
        {
            attendance.Status = AttendanceStatus.OnLeave;
        }
    }

    await _context.SaveChangesAsync();

    // Return simplified DTO
    return new LeaveRequestResponseDto()
    {
        Id = leave.Id,
        EmployeeId = leave.EmployeeId,
        EmployeeName = leave.Employee?.Name ?? "",
        LeaveType = leave.LeaveType,
        StartDate = leave.StartDate,
        EndDate = leave.EndDate,
        Reason = leave.Reason,
        LeaveSession = leave.LeaveSession,
        Status = leave.LeaveStatus,
        RequestedDate = leave.RequestDate,
        ApprovedAtDate = leave.ApprovedDate,
        ApprovedById = leave.ApprovedById,
        ApprovedByName = manager.Name
    };
}


    public async Task<LeaveRequestResponseDto> RejectAsync(int leaveRequestId, int managerId)
    {
        var leave = await _context.LeaveRequests
            .Include(l => l.Employee)
            .ThenInclude(e => e.Department)
            .FirstOrDefaultAsync(l => l.Id == leaveRequestId);

        if (leave == null)
            throw new Exception("Leave request not found");

        // Make sure the manager exists and has a department
        var manager = await _context.Employees
            .Include(m => m.Department)
            .FirstOrDefaultAsync(m => m.Id == managerId);

        if (manager == null)
            throw new Exception("Manager not found");

        if (manager.DepartmentId == null || leave.Employee?.DepartmentId == null)
            throw new Exception("Either manager or employee has no department assigned");

        // Only allow if manager is the head of employee's department
        if (leave.Employee.DepartmentId != manager.DepartmentId || manager.Id != leave.Employee.Department?.ManagerId)
            throw new UnauthorizedAccessException("Only the department manager can reject this leave request.");

        // Reject leave
        leave.LeaveStatus = LeaveStatus.Rejected;
        leave.RejectedDate = DateTime.UtcNow;
        leave.RejectedById = managerId;
        

        await _context.SaveChangesAsync();

        // Return DTO (no null reject reason here, only RejectedByName when status is rejected)
        return new LeaveRequestResponseDto()
        {
            Id = leave.Id,
            EmployeeId = leave.EmployeeId,
            EmployeeName = leave.Employee?.Name ?? "",
            LeaveType = leave.LeaveType,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            LeaveSession = leave.LeaveSession,
            Status = leave.LeaveStatus,
            RequestedDate = leave.RequestDate,
            RejectedAtDate = leave.RejectedDate,
            RejectedByName = manager.Name,
            RejectedById = managerId,
            RejectionReason = leave.RejectionReason
            
        };
    }

    public async Task<List<LeaveRequestResponseDto>> GetAllByEmployeeId(int employeeId, LeaveRequestFilter filter)
    {
        var query = _context.LeaveRequests
            .Where(l => l.EmployeeId == employeeId)  // only this employee
            .Include(l => l.Employee)
            .Include(l => l.ApprovedBy)
            .Include(l => l.RejectedBy)
            .AsQueryable();

        // Optional: filter by status
        if (filter.Status.HasValue)
            query = query.Where(l => l.LeaveStatus == filter.Status.Value);

        // Sorting
        var sortOrder = string.IsNullOrWhiteSpace(filter.SortOrder) ? "desc" : filter.SortOrder.ToLower();
        query = sortOrder == "asc"
            ? query.OrderBy(l => l.RequestDate)
            : query.OrderByDescending(l => l.RequestDate);

        // Pagination + projection
        var result = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(l => new LeaveRequestResponseDto()
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.Name,
                EmployeeAvatar = l.Employee.ImageUrl,
                LeaveType = l.LeaveType,
                LeaveSession = l.LeaveSession,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.LeaveStatus,
                RequestedDate = l.RequestDate,
                ApprovedAtDate = l.ApprovedDate,
                ApprovedByName = l.ApprovedBy != null ? l.ApprovedBy.Name : null,
                RejectedAtDate = l.RejectedDate,
                RejectedByName = l.RejectedBy != null ? l.RejectedBy.Name : null,
                RejectionReason = l.RejectionReason
            })
            .ToListAsync();

        return result;
    }
}