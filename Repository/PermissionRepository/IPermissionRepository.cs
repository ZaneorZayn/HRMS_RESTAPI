using hrms_api.Dto.PermissionDto;
using hrms_api.Model;

namespace hrms_api.Repository.PermissionRepository;

public interface IPermissionRepository
{
    Task<IEnumerable<GetPermissionDto>> GetAllPermissions();
    Task<Permission> GetPermissionById(int id);
    Task <CreatePermissionDto>AddPermission(CreatePermissionDto createPermissionDto);
    Task UpdatePermission(int id ,UpdatePermissionDto updatePermissionDto);
    Task DeletePermission(int id);
    Task AssignRoleToPermission(AssignPermissionDto assignPermissionDto);
}