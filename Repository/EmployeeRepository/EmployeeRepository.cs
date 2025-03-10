using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Model;
using hrms_api.Repository.EmailRepository;
using hrms_api.Repository.UserContext;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace hrms_api.Repository.EmployeeRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private readonly AppDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IEmailService _emailService;
        public EmployeeRepository(AppDbContext context, IUserContext userContext, IEmailService emailService)
        {

            _context = context;
            _userContext = userContext;
            _emailService = emailService;

        }
        public async Task AddAsync(EmployeeDto employeedto)
        {

            var role = _userContext.GetUserRole();
            if (role != "Admin" && role != "SuperAdmin")
            {
                throw new Exception("You do not have permission to add employees.");
            }

            var newEmployee = new Employee
            {
                Name = employeedto.Name,
                Email = employeedto.Email,
                Address = employeedto.Address,
                DOB = employeedto.DOB,
                HiredDate = employeedto.HiredDate

            };
            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();


        }




        public async Task DeleteAsync(int id)
        {

            var role = _userContext.GetUserRole();
            if (role != "Admin" && role != "SuperAdmin")
            {
                throw new Exception("You do not have permission to delete employees.");
            }


            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }
            else
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Employee>> GetAllAsync()
        {

            var role = _userContext.GetUserRole();
            if (role != "Admin" && role != "SuperAdmin")
            {
                throw new Exception("You do not have permission to view employees.");
            }

            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {

            var role = _userContext.GetUserRole();
            if (role != "Admin" && role != "SuperAdmin")
            {
                throw new Exception("You do not have permission to view employee.");
            }
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

            return employee;
        }

        public async Task LinkSystemUserAsync(int employeeId, int systemUserId)
        {

            var role = _userContext.GetUserRole();
            if (role != "Admin" && role != "SuperAdmin")
            {
                throw new Exception("You do not have permission to link employees to system users.");
            }

            var employee = await _context.Employees.FindAsync(employeeId);
            var user = await _context.SystemUsers.FindAsync(systemUserId);

            if (employee == null && user == null)
                throw new Exception("Employee and SystemUser are both not found!");

            if (employee == null)
                throw new Exception("Employee not found");

            if (user == null)
                throw new Exception("SystemUser not found");

            if (employee.SystemUserId != null)
                throw new Exception("Employee is already linked to a SystemUser");

            var alreadyLinkedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.SystemUserId == systemUserId);
            if (alreadyLinkedEmployee != null)
                throw new Exception("SystemUser is already linked to another Employee");

            employee.SystemUserId = systemUserId;
            await _context.SaveChangesAsync();

            string emailSubject = "Employee Linked to System User";
            string emailBody = $@"
            Dear {employee.Name},<br><br>
            Your HRMS account has been created. <br>
            <strong>Username:</strong> {user.Username}<br><br>
            Before logging in, you need to set your password.<br>
            Please request an OTP by entering your email and follow the instructions to reset your password.<br><br>
            Best Regards,<br>HR Team
        ";

            var emailRequest = new EmailDto
            {
                ToEmail = employee.Email,
                Subject = emailSubject,
                Body = emailBody
            };

            await _emailService.SendEmailAsync(emailRequest);

           
        }
    

        public async Task UpdateAsync(int id, EmployeeDto employeedto)
        {

            var role = _userContext.GetUserRole();
            if (role != "Admin" && role != "SuperAdmin")
            {
                throw new Exception("You do not have permission to update employees.");
            }
            var updateemployee = await _context.Employees.FindAsync(id);
            if (updateemployee == null)
            {
                throw new Exception("Employee not found");
            }


            updateemployee.Name = employeedto.Name;
            updateemployee.Email = employeedto.Email;
            updateemployee.Address = employeedto.Address;
            updateemployee.DOB = employeedto.DOB;
            updateemployee.HiredDate = employeedto.HiredDate;



                await _context.SaveChangesAsync();
            
        }

        public async Task UnlinkSystemUserAsync(int employeeId)
        {
            var role = _userContext.GetUserRole();

            if (role != "Admin" && role != "SuperAdmin")
            {
                throw new Exception("You do not have permission to unlink employees from system users.");
            }

            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
                throw new Exception("Employee not found");

            if (employee.SystemUserId == null)
                throw new Exception("Employee is not linked to a SystemUser");

            employee.SystemUserId = null;
            await _context.SaveChangesAsync();
        }

       
    }
}
