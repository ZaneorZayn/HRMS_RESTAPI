using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Model;
using hrms_api.Repository.EmailRepository;
using hrms_api.Repository.UserContext;
using Microsoft.EntityFrameworkCore;
using hrms_api.Helper;
using Microsoft.AspNetCore.Http.HttpResults;


namespace hrms_api.Repository.EmployeeRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {

        private readonly AppDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmployeeRepository (AppDbContext context, IUserContext userContext, IEmailService emailService, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {

            _context = context;
            _userContext = userContext;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task AddAsync(CreateEmployeeDto createEmployeeDto)
        {
            string? imagePath = createEmployeeDto.ImageUrl;

            // If image URL not provided, generate avatar
            if (string.IsNullOrEmpty(imagePath))
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var baseurl = $"{httpContext!.Request.Scheme}://{httpContext.Request.Host}";
                imagePath = Helper.AvatarGenerator.GenerateAvatar(createEmployeeDto.Name!, baseurl);
            }

            var employee = new Employee
            {
                Name = createEmployeeDto.Name,
                Email = createEmployeeDto.Email,
                Gender = createEmployeeDto.Gender,
                HiredDate = createEmployeeDto.HiredDate,
                DOB = createEmployeeDto.DOB,
                Address = createEmployeeDto.Address,
                PhoneNumber = createEmployeeDto.PhoneNumber,
                ImageUrl = imagePath
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(int id)
        {
            
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            // Delete employee image if it exists
            if (!string.IsNullOrEmpty(employee.ImageUrl))
            {
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
                var imageFileName = Path.GetFileName(employee.ImageUrl); // Extracts just the filename

                var imagePath = Path.Combine(uploadFolder, imageFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }


        public async Task<PagedResult<Employee>> GetAllAsync(QueryParameters parameters)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.Search))
            {
                string search = parameters.Search.ToLower();
                query = query.Where(e => e.Name!.Contains(search) || e.Email!.Contains(search));
            }

            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                query = parameters.SortOrder.ToLower() == "desc" 
                    ? query.OrderByDescending(q => q.Name)
                    : query.OrderBy(q => q.Name);
            }
            
            var totalCount = await query.CountAsync();
            
            var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync();

            return new PagedResult<Employee>
            {
                Data = items,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalItem = totalCount == 0 ? 0 : totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize),
            };
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

            return employee;
        }

        public async Task LinkSystemUserAsync(int employeeId, int systemUserId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            var user = await _context.SystemUsers.FindAsync(systemUserId);

            if (employee == null && user == null)
                throw new Exception("Employee and SystemUser are both not found!");

            if (employee == null || user == null)
                throw new Exception("Employee or SystemUser not found");
            
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
                ToEmail = employee.Email!,
                Subject = emailSubject,
                Body = emailBody
            };

            await _emailService.SendEmailAsync(emailRequest);

           
        }
    

        public async Task UpdateAsync(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

            // Update employee details
            employee.Name = updateEmployeeDto.Name;
            employee.Email = updateEmployeeDto.Email;
            employee.Gender = updateEmployeeDto.Gender;
            employee.Address = updateEmployeeDto.Address;
            employee.DOB = updateEmployeeDto.DOB;
            employee.HiredDate = updateEmployeeDto.HiredDate;
            employee.PhoneNumber = updateEmployeeDto.PhoneNumber;
            employee.ImageUrl = updateEmployeeDto.ImageUrl;
            
            await _context.SaveChangesAsync();
        }


        public async Task UnlinkSystemUserAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
                throw new Exception("Employee not found");

            if (employee.SystemUserId == null)
                throw new Exception("Employee is not linked to a SystemUser");

            employee.SystemUserId = null;
            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty");
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = $"{httpContext!.Request.Scheme}://{httpContext.Request.Host.Value}";

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
    
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);
    
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
    
            var imageUrl = $"{baseUrl}/Uploads/{uniqueFileName}"; // Full URL

            return imageUrl;
        }

    }
}
