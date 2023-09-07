using userMS.Application.DTOs;
using userMS.Application.DTOs.Request;

namespace userMS.Application.Services
{
    public interface IAuthService
    {
        Task<FirebaseAuthResponseDto> RegisterUserAsync(RegisterUserDto userReg);
        Task<bool> VerifyOtpAsync(OtpVerificationRequestDto otpVerificationRequestDto);
        Task<FirebaseAuthResponseDto> IdentifierLoginUserAsync(UsernameOrEmailLoginUserDto userLog);
        Task<FirebaseAuthResponseDto> PhoneLoginUserAsync(PhoneLoginUserDto userLog);
        Task<string> GetLoggedInEmailAsync(UsernameOrEmailLoginUserDto userLog);
        Task<string> GetLoggedInEmailAsync(PhoneLoginUserDto userLog);
    }
}
