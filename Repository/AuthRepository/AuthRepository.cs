using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace hrms_api.Repository.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
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

        public Task ValidateOtpAsync(string email, string otp)
        {
            throw new NotImplementedException();
        }

        
    }
}
