using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
namespace FInalProject.Services
{
    public interface IEmailService 
    {
        Task SendEmailFromServiceAsync(string toEmail, string subject, string body);
        Task<string> LoadEmailTemplateAsync(string TemplateName, Dictionary<string, string> placeholders);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> LoadEmailTemplateAsync(string templateName, Dictionary<string, string> placeholders)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", templateName);
            var content = await File.ReadAllTextAsync(templatePath);
            foreach (var placeholder in placeholders)
            {
                content = content.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }
            return content;
        }

        public async Task SendEmailFromServiceAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

    }
}
