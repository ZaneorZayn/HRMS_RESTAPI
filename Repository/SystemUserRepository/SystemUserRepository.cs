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
        public async Task AddAsync(SystemUserDto systemUserDto)
        {
            var loggedInUserRole = _userContext.GetUserRole();

            if (string.IsNullOrEmpty(loggedInUserRole))
            {
                throw new UnauthorizedAccessException("Unable to determine user role.");
            }

            loggedInUserRole = loggedInUserRole.Trim().ToLower(); // Normalize role string

            var hashpassword = BCrypt.Net.BCrypt.HashPassword(systemUserDto.Password);

            // Ensure the role exists
            var roleToCreate = await _context.Roles.FindAsync(systemUserDto.RoleId);
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
            
            var existUsername = await _context.SystemUsers.AnyAsync(u => u.Username == systemUserDto.Username);

            if (existUsername)
            {
                throw new Exception("Username already exists.");
            }
            
            var systemuser = new SystemUser
            {
                Username = systemUserDto.Username!,
                Password = hashpassword,
                RoleId = systemUserDto.RoleId,
            };

            _context.SystemUsers.Add(systemuser);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(int id)
        {
            var loggedInUserRole = _userContext.GetUserRole();

            if (string.IsNullOrEmpty(loggedInUserRole))
            {
                throw new UnauthorizedAccessException("Unable to determine user role.");
            }

            loggedInUserRole = loggedInUserRole.Trim().ToLower(); // Normalize role string

            var systemUserToDelete = await _context.SystemUsers.FindAsync(id);
            if (systemUserToDelete == null)
            {
                throw new Exception("SystemUser not found.");
            }

            // Get role of the user being deleted
            var roleToDelete = await _context.Roles.FindAsync(systemUserToDelete.RoleId);
            if (roleToDelete == null)
            {
                throw new Exception("Role associated with user not found.");
            }

            string roleToDeleteName = roleToDelete.Name.Trim().ToLower(); // Normalize role name

            // Apply role-based deletion rules
            if (loggedInUserRole == "admin" && roleToDeleteName != "user")
            {
                throw new UnauthorizedAccessException("Admins can only delete Employee accounts.");
            }

            if (loggedInUserRole != "superadmin" && loggedInUserRole != "admin")
            {
                throw new UnauthorizedAccessException("You do not have permission to delete users.");
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



        public async Task<List<SystemUser>> GetAllAsync()
        {
           var systemuser = await _context.SystemUsers.ToListAsync();

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


        public async Task UpdateAsync(int id, SystemUserDto systemUserDto)
        {
            var loggedInUserRole = _userContext.GetUserRole();

            if (string.IsNullOrEmpty(loggedInUserRole))
            {
                throw new UnauthorizedAccessException("Unable to determine user role.");
            }

            loggedInUserRole = loggedInUserRole.Trim().ToLower(); // Normalize role string

            var systemuser = await _context.SystemUsers.FindAsync(id);
            if (systemuser == null)
            {
                throw new Exception("SystemUser not found.");
            }

            // Get role of the user being updated
            var roleToUpdate = await _context.Roles.FindAsync(systemuser.RoleId);
            if (roleToUpdate == null)
            {
                throw new Exception("Role associated with user not found.");
            }

            string roleToUpdateName = roleToUpdate.Name.Trim().ToLower(); // Normalize role name

            // Role-based restrictions
            if (loggedInUserRole == "admin" && roleToUpdateName != "employee")
            {
                throw new UnauthorizedAccessException("Admins can only update Employee accounts.");
            }

            if (loggedInUserRole != "superadmin" && loggedInUserRole != "admin")
            {
                throw new UnauthorizedAccessException("You do not have permission to update users.");
            }

            // Update fields
            systemuser.Username = systemUserDto.Username;
            systemuser.Password = BCrypt.Net.BCrypt.HashPassword(systemUserDto.Password);
            systemuser.RoleId = systemUserDto.RoleId;

            _context.SystemUsers.Update(systemuser);
            await _context.SaveChangesAsync();
        }


    }
}
