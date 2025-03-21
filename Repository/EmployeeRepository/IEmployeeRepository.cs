using hrms_api.Dto;
using hrms_api.Model;

namespace hrms_api.Repository.EmployeeRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task<List<Employee>> GetAllAsync();
        Task AddAsync(CreateEmployeeDto createEmployeeDto);
        Task UpdateAsync(int id , UpdateEmployeeDto updateEmployeeDto);
        Task DeleteAsync(int id );
        Task LinkSystemUserAsync(int employeeId, int systemUserId);
        Task UnlinkSystemUserAsync(int employeeId);
    }
}
