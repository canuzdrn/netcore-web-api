using userMS.Application.DTOs;

namespace userMS.Application.Services
{
    public interface IAuthService
    {
        Task<RegisterUserDto> RegisterUserAsync(RegisterUserDto userReg);

        Task<LoginResponseDto> IdentifierLoginUserAsync(UsernameOrEmailLoginUserDto userLog);
        Task<LoginResponseDto> PhoneLoginUserAsync(PhoneLoginUserDto userLog);
        Task<string> GetLoggedInEmailAsync(UsernameOrEmailLoginUserDto userLog);
        Task<string> GetLoggedInEmailAsync(PhoneLoginUserDto userLog);
    }
}
