using hrms_api.Data;
using hrms_api.Dto.PermissionDto;
using hrms_api.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms_api.Repository.PermissionRepository;

public class PermissionRepository : IPermissionRepository

{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<GetPermissionDto>> GetAllPermissions()
    {
        var permission = await _context.Permissions.Select(p => new GetPermissionDto
        {
            PermissionId = p.PermissionId,
            PermissionName = p.PermissionName!,
        }).ToListAsync();
        
        return permission;
    }

    public async Task<Permission> GetPermissionById(int id)
    {
       var permission = await _context.Permissions.FindAsync(id);
       
       return permission!;
    }

    public async Task<CreatePermissionDto> AddPermission(CreatePermissionDto createPermissionDto)
    {
        var newPermission = new Permission
        {
            PermissionName = createPermissionDto.PermissionName,
        };
        _context.Permissions.Add(newPermission);
        await _context.SaveChangesAsync();

        return createPermissionDto;
    }

    public async Task UpdatePermission(int id ,UpdatePermissionDto updatePermissionDto)
    {
        var existingPermission =await _context.Permissions.FindAsync(id);
        if (existingPermission == null)
        {
            throw new Exception($"Permission with id {id} was not found");
        } 
        existingPermission.PermissionName = updatePermissionDto.PermissionName;
        
        await _context.SaveChangesAsync();
       
    }

    public async Task DeletePermission(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);

        if (permission == null) 
        {
            throw new Exception($"Permission with id {id} was not found");
        }
        
        _context.Permissions.Remove(permission);
        
        
    }

    public async Task AssignRoleToPermission(AssignPermissionDto assignPermissionDto)
    {
        var existingRole = await _context.Roles.
            Include(r => r.RolePermissions).
            FirstOrDefaultAsync(r =>r.Id == assignPermissionDto.RoleId);

        if (existingRole == null)
        {
            throw new Exception($"Role with id {assignPermissionDto.RoleId} was not found");
        }
        
        // Get existing permissions
        var existingPermissionIds = existingRole.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
    
        // Check if all new permissions already exist
        var alreadyAssigned = assignPermissionDto.PermissionIds.All(pid => existingPermissionIds.Contains(pid));
    
        if (alreadyAssigned)
        {
            throw new Exception("All selected permissions are already assigned to this role.");
        }

        // Add only new permissions (ignore existing ones)
        var newRolePermissions = assignPermissionDto.PermissionIds
            .Where(pid => !existingPermissionIds.Contains(pid)) // Only add if not already assigned
            .Select(pid => new RolePermission { RoleId = assignPermissionDto.RoleId, PermissionId = pid })
            .ToList();

        await _context.RolePermissions.AddRangeAsync(newRolePermissions);
        await _context.SaveChangesAsync();
    
    }
}

    