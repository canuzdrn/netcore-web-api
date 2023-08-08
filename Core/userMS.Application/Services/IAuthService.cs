using userMS.Application.DTOs;

namespace userMS.Application.Services
{
    public interface IAuthService
    {
        Task<RegisterUserDto> RegisterUserAsync(RegisterUserDto userReg);

        Task<LoginResponseDto> LoginUserAsync(LoginUserDto userLog);

        Task<string> GetLoggedInEmailAsync(LoginUserDto userLog);
    }
}
