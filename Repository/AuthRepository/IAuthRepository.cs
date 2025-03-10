using hrms_api.Dto;
using hrms_api.Model;
using Microsoft.AspNetCore.Mvc;

namespace hrms_api.Repository.AuthRepository
{
    public interface IAuthRepository
    {
        Task<LoginResponeDto> LoginAsync(LoginRequest loginRequest);
        Task <string> GenerateOtpAsync (string email);

        Task ValidateOtpAsync(string email, string otp);

        Task MarkOtpAsUsed(OtpRequest otpRequest);
        Task ResetPassword(ResetPasswordDto resetPasswordDto);

    }
}
