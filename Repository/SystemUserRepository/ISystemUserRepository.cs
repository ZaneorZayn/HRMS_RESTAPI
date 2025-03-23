using hrms_api.Dto;
using hrms_api.Model;

namespace hrms_api.Repository.SystemUserRepository
{
    public interface ISystemUserRepository
    {
        Task<SystemUser> GetByIdAsync(int id);
        Task<List<SystemUserGetDto>> GetAllAsync();
        Task AddAsync(SystemUserCreateDto systemUserCreateDto);
        Task UpdateAsync(int id ,SystemUserEditDto systemUserEditDto);
        Task DeleteAsync(int id);

    }
}
