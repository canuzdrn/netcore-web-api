using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using OtpNet;
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

        public async Task SendRegisterEmailAsync(UserRegisterMailRequestDto userRegisterMailRequestDto)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views/Register.html");
            var template = File.ReadAllText(templatePath);

            var verificationLink = $"https://localhost:7010/api/Auth/otp/verify/email?transactionId=" +
                $"{userRegisterMailRequestDto.Email}{userRegisterMailRequestDto.Otp}";

            var content = string.Format(template, userRegisterMailRequestDto.Username, verificationLink,
                userRegisterMailRequestDto.Otp);

            #region Email creation and sending
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(userRegisterMailRequestDto.Email));
            email.Subject = _options.EmailContent.RegisterSubject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = content
            };

            email.Body = bodyBuilder.ToMessageBody();

            var smtp = new SmtpClient();
            smtp.Connect(_options.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.EmailSettings.Username, _options.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();

            #endregion
        }

        public async Task SendLoginEmailAsync(UserLoginMailRequestDto userLoginMailRequestDto)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Views/Login.html");
            var template = File.ReadAllText(templatePath);

            var content = string.Format(template, userLoginMailRequestDto.Username, DateTime.Now.ToString());

            #region Email creation and sending

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(userLoginMailRequestDto.Email));
            email.Subject = _options.EmailContent.LoginSubject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = content
            };

            email.Body = bodyBuilder.ToMessageBody();

            var smtp = new SmtpClient();
            smtp.Connect(_options.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.EmailSettings.Username, _options.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();

            #endregion
        }
        public async Task SendCustomEmailAsync(EmailSendRequestDto emailSendRequestDto)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.EmailSettings.Username));
            email.To.Add(MailboxAddress.Parse(emailSendRequestDto.To));
            email.Subject = emailSendRequestDto.Subject ?? "" ;
            //email.Body = new TextPart(TextFormat.Html) { Text = (emailSendRequestDto.Body + "<br/>" + $" {DateTime.Now} ") };

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailSendRequestDto.Body
            };

            email.Body = bodyBuilder.ToMessageBody();

            var smtp = new SmtpClient();
            smtp.Connect(_options.EmailSettings.Host, PortTLS, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.EmailSettings.Username, _options.EmailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }
    }
}
