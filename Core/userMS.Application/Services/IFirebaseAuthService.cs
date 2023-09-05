using userMS.Application.DTOs;
using userMS.Application.DTOs.Response;

namespace userMS.Application.Services
{
    public interface IFirebaseAuthService
    {
        Task<FirebaseLoginResponseDto> FirebaseLoginAsync(FirebaseRequestDto request);

        Task<FirebaseRegisterResponseDto> FirebaseRegisterAsync(FirebaseRequestDto request);
    }
}
