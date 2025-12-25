using System.ComponentModel.DataAnnotations;
using hrms_api.Enum;

namespace hrms_api.Model;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [MaxLength(150)]
    public string? Reason { get; set; }
    public LeaveSession LeaveSession { get; set; } = LeaveSession.FullDay;
    public LeaveStatus LeaveStatus { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public int? ApprovedById { get; set; }
    public Employee? ApprovedBy { get; set; }
    
    public DateTime? RejectedDate { get; set; }
    
    public int? RejectedById { get; set; }
    
    public Employee? RejectedBy { get; set; }
    
    [MaxLength(150)]
    public string? RejectionReason { get; set; } 
}