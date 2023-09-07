using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using userMS.Application.DTOs.Request;
using userMS.Application.Services;
using userMS.Infrastructure.Com;

namespace userMS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _options;

        private const int PortTLS = 587;

        public EmailService(IOptions<AppSettings> options)
        {
            _options = options.Value;
        }

        public async Task SendRegisterEmailAsync(string to)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = _options.EmailContent.RegisterSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = _options.EmailContent.RegisterBody };

            var smtp = new SmtpClient();
            smtp.Connect(_options.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.EmailSettings.Username, _options.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }

        public async Task SendLoginEmailAsync(string to)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = _options.EmailContent.LoginSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = (_options.EmailContent.LoginBody + "<br/>" + $" {DateTime.Now} ") };

            var smtp = new SmtpClient();
            smtp.Connect(_options.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.EmailSettings.Username, _options.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }
        public async Task SendCustomEmailAsync(EmailSendRequestDto emailSendRequestDto)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(emailSendRequestDto.To));
            email.Subject = emailSendRequestDto.Subject ?? "" ;
            email.Body = new TextPart(TextFormat.Html) { Text = (emailSendRequestDto.Body + "<br/>" + $" {DateTime.Now} ") };

            var smtp = new SmtpClient();
            smtp.Connect(_options.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.EmailSettings.Username, _options.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }

    }
}
