using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using hrms_api.Dto.AuthenticationDto;
using hrms_api.Helper;
using hrms_api.Repository.EmailRepository;
using hrms_api.Repository.UserContext;

namespace hrms_api.Repository.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {

        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IUserContext _userContext;
        private readonly IEmailService _emailService;

        public AuthRepository(AppDbContext context, IConfiguration configuration, IUserContext userContext , IEmailService emailService, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _userContext = userContext;
            _emailService = emailService;
        }

        public async Task<string> GenerateOtpAsync (string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var existingOtp = await _context.OtpRequests
                .Where(x => x.email == email && !x.IsUsed)
                .ToListAsync();

            // Remove any existing unused OTPs
            _context.OtpRequests.RemoveRange(existingOtp);

            // Add new OTP request
            var otpRequest = new OtpRequest
            {
                email = email,
                otp = otp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _context.OtpRequests.AddAsync(otpRequest);
            await _context.SaveChangesAsync();

            // Send OTP email
            var emailBody = $"Your OTP for password reset is: <strong>{otp}</strong>. It is valid for 5 minutes.";
            await _emailService.SendEmailAsync(new EmailDto
            {
                ToEmail = email,
                Subject = "Password Reset OTP",
                Body = emailBody
            });

            return otp;
        }


        public async Task<LoginResponeDto> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _context.SystemUsers.
                Include(x => x.Role).
                Include(x => x.Role!.RolePermissions).
                ThenInclude(rp => rp.Permission).
                FirstOrDefaultAsync(x => x.Username == loginRequest.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                throw new Exception("Invalid username or password");
            }

            if (user.Role == null)
            {
                throw new Exception("User role not found.");
            }
            var permissions = user.Role.RolePermissions.Select(x => x.Permission!.PermissionName).ToList();
            var tokenString = _jwtService.GenerateToken(user, permissions!);

            var refreshToken = _jwtService.GenerateRefeshToken();

            var refreshTokenEntity = new RefreshToken
            {
                refreshToken = refreshToken,
                expires = DateTime.UtcNow.AddDays(7),
                systemuserId = user.Id,

            }; 
            
            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();
            
            return new LoginResponeDto
            {
               TokenType = "Bearer",
               Token = tokenString,
               Expiration = DateTime.UtcNow.AddMinutes(30),
               RefreshToken = refreshToken,
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

            _context.OtpRequests.Remove(otpRequest);

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
                    Role = e.SystemUser.Role!.Name
                })
                .FirstOrDefaultAsync();

            if (profile != null) return profile;
            // If no linked employee found, return only SystemUser details (Username and Role)
            var systemUser = await _context.SystemUsers
                .Where(su => su.Id == systemUserId)
                .Select(su => new ProfileDto
                {
                    SystemUserId = su.Id,
                    Username = su.Username,
                    Role = su.Role!.Name
                })
                .FirstOrDefaultAsync();

            return systemUser!;

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
            employee.ImageUrl = editProfileDto.Image;
            
            _context.Update(employee);
            await _context.SaveChangesAsync();
            return editProfileDto;
        }

        public async Task<RefreshTokenDto> GetRefreshToken(string refreshToken)
        {
            var tokenStore = await _context.RefreshTokens.
                Include(rt => rt.systemUser).
                ThenInclude(r => r.Role).
                ThenInclude(rp => rp.RolePermissions).
                ThenInclude( pt => pt.Permission).
                FirstOrDefaultAsync(rt => rt.refreshToken == refreshToken);

            if (tokenStore == null || tokenStore.expires < DateTime.UtcNow)
            {
                throw new Exception("Invalid refresh token");
            }
            
            var user  = tokenStore.systemUser;

            if (user == null || user.Role == null)
            {
                throw new Exception("User not found ");
            }
            var permissions = user.Role.RolePermissions
                .Select(rp => rp.Permission!.PermissionName)
                .ToList();

            // Generate new access token
            var newAccessToken = _jwtService.GenerateToken(user, permissions);

            // Rotate refresh token (optional but recommended)
            var newRefreshToken = _jwtService.GenerateRefeshToken();
            tokenStore.refreshToken = newRefreshToken;
            
            tokenStore.expires = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new RefreshTokenDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = tokenStore.refreshToken,
                Expiration = tokenStore.expires,
                TokenType = "Bearer",
                
            };
        }

        public Task ValidateOtpAsync(string email, string otp)
        {
            throw new NotImplementedException();
        }

        
    }
}
