using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using userMS.Application.Services;
using userMS.Infrastructure.Com;

namespace userMS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly IOptions<EmailContent> _emailContent;

        private const int PortTLS = 587;

        public EmailService(IOptions<EmailSettings> emailSettings, IOptions<EmailContent> emailContent)
        {
            _emailSettings = emailSettings;
            _emailContent = emailContent;
        }

        public async Task SendRegisterEmailAsync(string to)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.Value.Username));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = _emailContent.Value.RegisterSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = _emailContent.Value.RegisterBody };

            var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Value.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Value.Username, _emailSettings.Value.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }

        public async Task SendLoginEmailAsync(string to)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.Value.Username));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = _emailContent.Value.LoginSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = (_emailContent.Value.LoginBody + "<br/>" + $" {DateTime.Now} ") };

            var smtp = new SmtpClient();
            smtp.Connect(_emailSettings.Value.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.Value.Username, _emailSettings.Value.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }
    }
}
