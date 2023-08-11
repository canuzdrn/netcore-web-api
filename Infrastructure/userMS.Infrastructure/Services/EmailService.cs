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
        private readonly IOptions<AppSettings> _options;

        private const int PortTLS = 587;

        public EmailService(IOptions<AppSettings> options)
        {
            _options = options;
        }

        public async Task SendRegisterEmailAsync(string to)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.Value.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = _options.Value.EmailContent.RegisterSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = _options.Value.EmailContent.RegisterBody };

            var smtp = new SmtpClient();
            smtp.Connect(_options.Value.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Value.EmailSettings.Username, _options.Value.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }

        public async Task SendLoginEmailAsync(string to)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.Value.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = _options.Value.EmailContent.LoginSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = (_options.Value.EmailContent.LoginBody + "<br/>" + $" {DateTime.Now} ") };

            var smtp = new SmtpClient();
            smtp.Connect(_options.Value.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Value.EmailSettings.Username, _options.Value.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }
    }
}
