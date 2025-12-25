namespace hrms_api.Dto.AttendanceDto;

public class AttendanceScanDto
{
    public string QrContent { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}