using System.Text.Json.Serialization;
using hrms_api.Enum;

namespace hrms_api.Model;

public class Attendance
{
    public int Id { get; set; }
    
    public int SystemUserId { get; set; }
    public int EmployeeId { get; set; }
    
    public DateTime Date { get; set; }
    public TimeSpan ClockIn { get; set; }
    public TimeSpan? ClockOut { get; set; }
    public TimeSpan TotalHours { get; set; }
    public TimeSpan OverTime { get; set; }
    public AttendanceStatus Status { get; set; }
    [JsonIgnore]
    public SystemUser? SystemUser { get; set; }
    [JsonIgnore]
    public Employee? Employee { get; set; }
}