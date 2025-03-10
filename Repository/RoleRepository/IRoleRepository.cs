using hrms_api.Dto;
using hrms_api.Model;

namespace hrms_api.Repository.RoleRepository
{
    public interface IRoleRepository
    {

        Task AddAsync(RoleDto roledto);
        Task DeleteAsync(int id);
        Task<List<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(int id);

        Task UpdateAsync(int id, RoleDto roleDto);
    }
}
