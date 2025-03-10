using hrms_api.Dto;
using hrms_api.Model;

namespace hrms_api.Repository.EmployeeRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task<List<Employee>> GetAllAsync();
        Task AddAsync(EmployeeDto employeedto);
        Task UpdateAsync(int id , EmployeeDto employeedto);
        Task DeleteAsync(int id );
        Task LinkSystemUserAsync(int employeeId, int systemUserId);
        Task UnlinkSystemUserAsync(int employeeId);
    }
}
