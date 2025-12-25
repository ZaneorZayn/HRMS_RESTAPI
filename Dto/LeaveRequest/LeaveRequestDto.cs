using System.ComponentModel.DataAnnotations.Schema;
using hrms_api.Enum;

namespace hrms_api.Dto.LeaveRequest;

public class LeaveRequestDto
{
    
    public LeaveType LeaveType { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveSession LeaveSession { get; set; } = LeaveSession.FullDay;
}

public class LeaveRequestResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    
    public string? EmployeeAvatar { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string? EmployeeEmail { get; set; }

    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveSession LeaveSession { get; set; }
    public LeaveStatus Status { get; set; }

    public DateTime? RequestedDate { get; set; }
    public DateTime? ApprovedAtDate { get; set; }
    public int? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? RejectedAtDate { get; set; }
    
    public int? RejectedById { get; set; }
    
    public string? RejectedByName { get; set; }
    
    public string? RejectionReason { get; set; }
}

public class LeaveRequestFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public LeaveStatus? Status { get; set; } 
    public string? SortOrder { get; set; }
}