using userMS.Application.DTOs;
using userMS.Application.DTOs.Request;
using userMS.Application.DTOs.Response;

namespace userMS.Application.Services
{
    public interface IFirebaseAuthService
    {
        Task<FirebaseAuthResponseDto> FirebaseEmailLoginAsync(FirebaseEmailSignInRequestDto request);
        Task<FirebaseAuthResponseDto> FirebasePhoneLoginAsync(FirebasePhoneSignInRequestDto request);
        Task<FirebaseAuthResponseDto> FirebaseRegisterAsync(FirebaseEmailSignInRequestDto request);
        Task<OauthVerificationResponseDto> FirebaseOauthLoginAsync(OauthVerificationRequestDto googleVerificationRequestDto);
    }
}
