using System.ComponentModel.DataAnnotations;
using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Dto.AuthenticationDto;
using hrms_api.Repository.AuthRepository;
using hrms_api.Repository.EmailRepository;
using hrms_api.Repository.UserContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hrms_api.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IUserContext _userContext;

        public AuthController(IAuthRepository authrepo, AppDbContext appDbContext, IEmailService emailService,IUserContext userContext)
        {
            _authRepository = authrepo;
            _context = appDbContext;
            _emailService = emailService;
            _userContext = userContext;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var response = await _authRepository.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto request)
        {
            
            if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
            {
                return BadRequest("Invalid email format.");
            }
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (employee == null)
            {
                return BadRequest("No account found with this email.");
            }

            var otp = await _authRepository.GenerateOtpAsync(request.Email);

            var emailBody = $"Your OTP for password reset is: <strong>{otp}</strong>. It is valid for 5 minutes.";

            await _emailService.SendEmailAsync(new EmailDto
            {
                ToEmail = request.Email,
                Subject = "Password Reset OTP",
                Body = emailBody
            });

            return Ok("OTP has been sent to your email.");
        }

        [HttpPost("reset-password")]

        public async Task<IActionResult> Resetpassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
              await _authRepository.ResetPassword(resetPasswordDto);
                return Ok(new { message = "Password reset successfuly" });

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {   
                var userId = _userContext.GetUserId();
                var profile = await _authRepository.GetProfile(userId);
                
                return Ok(profile);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost("edit-profile")]

        public async Task<IActionResult> EditProfile([FromForm] EditProfileDto editProfileDto)
        {
            try
            {
               
                var editProfile = await _authRepository.EditProfile( editProfileDto);
                
                return Ok(new { message = " Update profile successfuly", data = editProfile });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
