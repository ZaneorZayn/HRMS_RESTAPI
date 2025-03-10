using hrms_api.Dto;
using hrms_api.Model;

namespace hrms_api.Repository.SystemUserRepository
{
    public interface ISystemUserRepository
    {
        Task<SystemUser> GetByIdAsync(int id);
        Task<List<SystemUser>> GetAllAsync();
        Task AddAsync(SystemUserDto systemUserDto);
        Task UpdateAsync(int id ,SystemUserDto systemUserDto);
        Task DeleteAsync(int id);

    }
}
