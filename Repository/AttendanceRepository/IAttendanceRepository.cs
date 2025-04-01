using hrms_api.Dto.AttendanceDto;
using hrms_api.Model;

namespace hrms_api.Repository.AttendanceRepository;

public interface IAttendanceRepository
{
    Task MarkCheckInAsync(int systemUserId);
    Task MarkCheckOutAsync(int systemUserId);
    Task<List<Attendance>> GetAttendancesByToday();
}