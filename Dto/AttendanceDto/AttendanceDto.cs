namespace hrms_api.Dto.AttendanceDto;

public class AttendanceDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public TimeSpan? ClockIn { get; set; }
    public TimeSpan? ClockOut { get; set; }
}