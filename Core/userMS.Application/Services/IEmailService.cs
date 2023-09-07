using userMS.Application.DTOs.Request;

namespace userMS.Application.Services
{
    public interface IEmailService
    {
        Task SendRegisterEmailAsync(string to);

        Task SendLoginEmailAsync(string to);

        Task SendCustomEmailAsync(EmailSendRequestDto emailSendRequestDto);
    }
}
