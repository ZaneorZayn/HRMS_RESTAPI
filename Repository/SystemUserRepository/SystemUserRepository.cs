using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Model;
using hrms_api.Repository.UserContext;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace hrms_api.Repository.SystemUserRepository
{
    public class SystemUserRepository : ISystemUserRepository
    {

        private readonly AppDbContext _context;
        private readonly IUserContext _userContext;

        public SystemUserRepository(AppDbContext context, IUserContext userContext)
        {

            _context = context;
            _userContext = userContext;
        }
        public async Task AddAsync(SystemUserCreateDto systemUserCreateDto)
        {
            var loggedInUserRole = _userContext.GetUserRole();

            if (string.IsNullOrEmpty(loggedInUserRole))
            {
                throw new UnauthorizedAccessException("Unable to determine user role.");
            }

            loggedInUserRole = loggedInUserRole.Trim().ToLower(); // Normalize role string

            var hashpassword = BCrypt.Net.BCrypt.HashPassword(systemUserCreateDto.Password);

            // Ensure the role exists
            var roleToCreate = await _context.Roles.FindAsync(systemUserCreateDto.RoleId);
            if (roleToCreate == null)
            {
                throw new Exception("Role not found.");
            }

            string roleToCreateName = roleToCreate.Name.Trim().ToLower(); // Normalize

            // Restrict Admins to only creating User accounts
            if (loggedInUserRole == "admin" && roleToCreateName != "user")
            {
                throw new UnauthorizedAccessException("Admins can only create User accounts.");
            }

            // Only SuperAdmin and Admin can create users
            if (loggedInUserRole != "superadmin" && loggedInUserRole != "admin")
            {
                throw new UnauthorizedAccessException("You do not have permission to create users.");
            }
            //check if username exist
            
            var existUsername = await _context.SystemUsers.AnyAsync(u => u.Username == systemUserCreateDto.Username);

            if (existUsername)
            {
                throw new Exception("Username already exists.");
            }
            
            var systemuser = new SystemUser
            {
                Username = systemUserCreateDto.Username!,
                Password = hashpassword,
                RoleId = systemUserCreateDto.RoleId,
                
            };

            _context.SystemUsers.Add(systemuser);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(int id)
        {
            var systemUserToDelete = await _context.SystemUsers.FindAsync(id);
            if (systemUserToDelete == null)
            {
                throw new Exception("SystemUser not found.");
            }

            // Unlink SystemUser from Employee (if linked)
            var linkedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.SystemUserId == id);
            if (linkedEmployee != null)
            {
                linkedEmployee.SystemUserId = null;
            }

            _context.SystemUsers.Remove(systemUserToDelete);
            await _context.SaveChangesAsync();
        }

        
        public async Task<List<SystemUserGetDto>> GetAllAsync()
        {
            var systemuser = await _context.SystemUsers.Include(s => s.Role).Select(su => new SystemUserGetDto
            {
                Id = su.Id,
                Username = su.Username,
                RoleId = su.RoleId,
                RoleName = su.Role != null ? su.Role.Name : "N/A"
            }).ToListAsync();

            return systemuser;
        }

        public async Task<SystemUser> GetByIdAsync(int id)
        {
            var systemuser = await _context.SystemUsers.FindAsync(id);
          
            if (systemuser == null)
            {
                throw new Exception("SystemUser not found");
            }

            return systemuser;
        }


        public async Task UpdateAsync(int id, SystemUserEditDto systemUserEditDto)
        {
           

            var systemuser = await _context.SystemUsers.FindAsync(id);
            if (systemuser == null)
            {
                throw new Exception("SystemUser not found.");
            }

            // Get role of the user being updated

            // Update fields
            systemuser.Username = systemUserEditDto.Username;
            //systemuser.Password = BCrypt.Net.BCrypt.HashPassword(systemUserDto.Password);
            systemuser.RoleId = systemUserEditDto.RoleId;

            _context.SystemUsers.Update(systemuser);
            await _context.SaveChangesAsync();
        }


    }
}
