using hrms_api.Data;
using hrms_api.Dto;
using hrms_api.Repository.AuthRepository;
using hrms_api.Repository.EmailRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hrms_api.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authrepo;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(IAuthRepository authrepo, AppDbContext appDbContext, IEmailService emailService)
        {
            _authrepo = authrepo;
            _context = appDbContext;
            _emailService = emailService;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var response = await _authrepo.LoginAsync(loginRequest);
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
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (employee == null)
            {
                return BadRequest("No account found with this email.");
            }

            var otp = await _authrepo.GenerateOtpAsync(request.Email);

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
              await _authrepo.ResetPassword(resetPasswordDto);
                return Ok(new { message = "Password reset successfuly" });

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
