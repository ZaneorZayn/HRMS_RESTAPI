using hrms_api.Repository.EmailRepository;
using System.Net.Mail;
using System.Net;
using hrms_api.Dto;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(EmailDto request)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");

        string host = smtpSettings["Host"];
        string portString = smtpSettings["Port"];
        string username = smtpSettings["Username"];
        string password = smtpSettings["Password"];
        string fromEmail = smtpSettings["FromEmail"];
        string enableSslString = smtpSettings["EnableSsl"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portString) || string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(enableSslString))
        {
            throw new Exception("SMTP settings are missing in configuration.");
        }

        int port = int.Parse(portString);
        bool enableSsl = bool.Parse(enableSslString);

        var smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = request.Subject,
            Body = request.Body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(request.ToEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
