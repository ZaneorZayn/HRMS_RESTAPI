using hrms_api.Dto.LeaveRequest;
using hrms_api.Model;


namespace hrms_api.Repository.LeaveRequestRepository;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(int id);
    Task<List<LeaveRequestResponseDto>> GetAllAsync(LeaveRequestFilter filter);
    Task<int> CountAsync(LeaveRequestFilter filter);
    Task<LeaveRequest> CreateAsync(LeaveRequestDto leave , int employeeId);
    Task<LeaveRequestResponseDto> ApproveAsync(int leaveRequestId, int managerId);
    Task<LeaveRequestResponseDto> RejectAsync(int leaveRequestId, int managerId);
    
    Task<List<LeaveRequestResponseDto>> GetAllByEmployeeId(int employeeId , LeaveRequestFilter filter);
    
}