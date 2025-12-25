using hrms_api.Dto;
using hrms_api.Helper;
using hrms_api.Model;

namespace hrms_api.Repository.EmployeeRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task<PagedResult<Employee>> GetAllAsync( QueryParameters parameters);
        Task AddAsync(CreateEmployeeDto createEmployeeDto);
        Task UpdateAsync(int id , UpdateEmployeeDto updateEmployeeDto);
        Task DeleteAsync(int id );
        Task LinkSystemUserAsync(int employeeId, int systemUserId);
        Task UnlinkSystemUserAsync(int employeeId);
        
        Task<string> UploadImage (IFormFile file);
    }
}
