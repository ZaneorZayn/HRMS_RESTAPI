using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms_api.Repository.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {

        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {

            _context = context;
        }
        public async Task AddAsync(RoleDto roleDto)
        {
            var newRole = new Role
            {
                Name = roleDto.Name!

            };
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                throw new Exception("Role not found");
            }
            return role;
        }

        public async Task UpdateAsync(int id, RoleDto roleDto)
        {
            var updateRole = await _context.Roles.FindAsync(id);

            if(updateRole == null)
            {
                throw new Exception("Role not found");
            }
            else
            {
                updateRole.Name = roleDto.Name!;
            }
            _context.Roles.Update(updateRole);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if(role == null)
            {
                throw new Exception("ID not found");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }


    }
}
