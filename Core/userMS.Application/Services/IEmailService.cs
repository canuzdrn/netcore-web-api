using userMS.Application.DTOs.Request;

namespace userMS.Application.Services
{
    public interface IEmailService
    {
        Task SendRegisterEmailAsync(UserRegisterMailRequestDto userRegisterMailRequestDto);

        Task SendLoginEmailAsync(UserLoginMailRequestDto userLoginMailRequestDto);

        Task SendCustomEmailAsync(EmailSendRequestDto emailSendRequestDto);
    }
}
