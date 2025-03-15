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

        string host = smtpSettings["Host"] ?? throw new Exception("SMTP Host is missing.");
        string portString = smtpSettings["Port"] ?? throw new Exception("SMTP Port is missing.");
        string username = smtpSettings["Username"] ?? throw new Exception("SMTP Username is missing.");
        string password = smtpSettings["Password"] ?? throw new Exception("SMTP Password is missing.");
        string fromEmail = smtpSettings["FromEmail"] ?? throw new Exception("SMTP FromEmail is missing.");
        string enableSslString = smtpSettings["EnableSsl"] ?? "true"; // Default to true

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