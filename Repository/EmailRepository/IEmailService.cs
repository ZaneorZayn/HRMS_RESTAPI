using hrms_api.Dto;

namespace hrms_api.Repository.EmailRepository
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto request);
    }
}
