﻿using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using hrms_api.Dto.AuthenticationDto;
using hrms_api.Repository.UserContext;

namespace hrms_api.Repository.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserContext _userContext;

        public AuthRepository(AppDbContext context, IConfiguration configuration, IUserContext userContext)
        {
            _context = context;
            _configuration = configuration;
            _userContext = userContext;
        }

        public async Task <string> GenerateOtpAsync(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var existingOtp = await _context.OtpRequests
                .Where(x => x.email == email && !x.IsUsed)
                .ToListAsync();

            _context.OtpRequests.RemoveRange(existingOtp);

            var otpRequest = new OtpRequest
            {
                email = email,
                otp = otp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _context.OtpRequests.AddAsync(otpRequest);
            await _context.SaveChangesAsync();

            return otp;

        }

        public async Task<LoginResponeDto> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _context.SystemUsers.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == loginRequest.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                throw new Exception("Invalid username or password");
            }

            if (user.Role == null)
            {
                throw new Exception("User role not found.");
            }

            // Generate token 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var expiration = DateTime.UtcNow.AddHours(2);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Name)
                }),
                Expires = expiration,
                Issuer = _configuration["Jwt:Issuer"],  // Add issuer
                Audience = _configuration["Jwt:Audience"], // Add audience
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new LoginResponeDto
            {
                Token = tokenString,
                Expiration = expiration,
            };
        }

        public Task MarkOtpAsUsed(OtpRequest otpRequest)
        {
            throw new NotImplementedException();
        }

        public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var otpRequest = await _context.OtpRequests
               .FirstOrDefaultAsync(x => x.otp == resetPasswordDto.Otp && !x.IsUsed);

            if (otpRequest == null || otpRequest.ExpirationTime< DateTime.UtcNow)
            {
                throw new Exception("Invalid OTP or OTP expired");
            }

            var employee = await _context.Employees
                .Include(e => e.SystemUser)//Include systemuser
                .FirstOrDefaultAsync(e => e.Email == otpRequest.email);

            if (employee == null || employee.SystemUser == null)
            {
                throw new Exception("User not found ");
            }

            var user = employee.SystemUser;

            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);

            otpRequest.IsUsed = true;

            await _context.SaveChangesAsync();
        }

        public async Task<ProfileDto> GetProfile(int systemUserId)
        {
            // Try to get the employee associated with the system user
            var profile = await _context.Employees
                .Where(e => e.SystemUserId == systemUserId)
                .Select(e => new ProfileDto
                {
                    EmployeeId = e.Id,
                    Name = e.Name!,
                    Email = e.Email!,
                    Address = e.Address!,
                    DOB = e.DOB,
                    HiredDate = e.HiredDate,
                    PhoneNumber = e.PhoneNumber!,
                    ImageUrl = e.ImageUrl!,
                    SystemUserId = e.SystemUserId ?? 0,
                    Username = e.SystemUser!.Username,
                    Role = e.SystemUser.Role.Name
                })
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                // If no linked employee found, return only SystemUser details (Username and Role)
                var systemUser = await _context.SystemUsers
                    .Where(su => su.Id == systemUserId)
                    .Select(su => new ProfileDto
                    {
                        SystemUserId = su.Id,
                        Username = su.Username,
                        Role = su.Role.Name
                    })
                    .FirstOrDefaultAsync();

                return systemUser!;
            }

            return profile;
        }


        public async Task<EditProfileDto> EditProfile(EditProfileDto editProfileDto)
        {

            var systemUserId = _userContext.GetUserId();
            var employee =  await _context.Employees
                .Include(e => e.SystemUser) // Include SystemUser details
                .FirstOrDefaultAsync(e => e.SystemUserId == systemUserId);

            if (employee == null)
            {
                throw new KeyNotFoundException("Employee not found.");
            }

            // Update profile details
            
            employee.Name = editProfileDto.Name;
            employee.Email = editProfileDto.Email;
            employee.Address = editProfileDto.Address;
            employee.DOB = editProfileDto.DOB;
            employee.PhoneNumber = editProfileDto.PhoneNumber;
            
            if (editProfileDto.Image != null)
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload");

                // Ensure the directory exists
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(editProfileDto.Image.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await editProfileDto.Image.CopyToAsync(stream);
                }

                // Update employee image URL
                employee.ImageUrl = $"/upload/{fileName}"; // Use correct URL path
            }

            _context.Update(employee);
            await _context.SaveChangesAsync();
            return editProfileDto;
        }

        public Task ValidateOtpAsync(string email, string otp)
        {
            throw new NotImplementedException();
        }

        
    }
}
