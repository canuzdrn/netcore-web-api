using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using userMS.Application.DTOs;
using userMS.Application.DTOs.Response;
using userMS.Application.Services;
using userMS.Domain.Exceptions;
using userMS.Infrastructure.Com;
using userMS.Infrastructure.Statics;

namespace userMS.Persistence.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly AppSettings _options;
        private readonly HttpClient _httpClient;

        public FirebaseAuthService(IOptions<AppSettings> options, HttpClient httpClient)
        {
            _options = options.Value;
            _httpClient = httpClient;
        }

        public async Task<FirebaseLoginResponseDto> FirebaseLoginAsync(FirebaseRequestDto request)
        {
            var requestUri = new Uri($"{_options.IdentityToolkitBaseUrl}/v1/accounts:signInWithPassword?key={_options.FirebaseApiKey}");

            var response = await _httpClient.PostAsJsonAsync(requestUri, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>();
                if (errorResponse is null) throw new BadRequestException(ErrorMessages.FirebaseLoginError);

                throw new BadRequestException(errorResponse.Error.Message);
            }

            var responseData = await response.Content.ReadFromJsonAsync<FirebaseLoginResponseDto>();

            if (responseData is null) { /* TODO */ throw new Exception(); }

            return responseData;
        }

        public async Task<FirebaseRegisterResponseDto> FirebaseRegisterAsync(FirebaseRequestDto request)
        {
            var requestUri = new Uri($"{_options.IdentityToolkitBaseUrl}/v1/accounts:signUp?key={_options.FirebaseApiKey}");

            var response = await _httpClient.PostAsJsonAsync(requestUri,request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>();
                if (errorResponse is null) throw new BadRequestException(ErrorMessages.FirebaseRegisterError);

                throw new BadRequestException(errorResponse.Error.Message);
            }

            var responseData = await response.Content.ReadFromJsonAsync<FirebaseRegisterResponseDto>();

            if (responseData is null) { /* TODO */ throw new Exception();  }

            return responseData;
        }
    }
}
