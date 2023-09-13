using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using userMS.Application.DTOs;
using userMS.Application.DTOs.Request;
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

        public async Task<FirebaseAuthResponseDto> FirebaseEmailLoginAsync(FirebaseEmailSignInRequestDto request)
        {
            var requestUri = new Uri($"{_options.IdentityToolkitBaseUrl}:signInWithPassword?key={_options.FirebaseApiKey}");

            var response = await _httpClient.PostAsJsonAsync(requestUri, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<FirebaseErrorResponseDto>();

                throw new BadRequestException(errorResponse.Error.Message);
            }

            var responseData = await response.Content.ReadFromJsonAsync<FirebaseAuthResponseDto>();

            if(responseData is null) throw new BadRequestException(ErrorMessages.FirebaseLoginError);

            return responseData;
        }

        public async Task<FirebaseAuthResponseDto> FirebaseRegisterAsync(FirebaseEmailSignInRequestDto request)
        {
            var requestUri = new Uri($"{_options.IdentityToolkitBaseUrl}:signUp?key={_options.FirebaseApiKey}");

            var response = await _httpClient.PostAsJsonAsync(requestUri,request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<FirebaseErrorResponseDto>();

                throw new BadRequestException(errorResponse.Error.Message);
            }

            var responseData = await response.Content.ReadFromJsonAsync<FirebaseAuthResponseDto>();

            if (responseData is null) throw new BadRequestException(ErrorMessages.FirebaseRegisterError);

            return responseData;
        }

        public async Task<FirebaseAuthResponseDto> FirebasePhoneLoginAsync(FirebasePhoneSignInRequestDto request)
        {
            #region send verification code
            var verificationRequestUri = 
                new Uri($"{_options.IdentityToolkitBaseUrl}:sendVerificationCode?key={_options.FirebaseApiKey}");

            var verificationResponse = await _httpClient.PostAsJsonAsync(verificationRequestUri, request);

            if (!verificationResponse.IsSuccessStatusCode)
            {
                var errorResponse = await verificationResponse.Content.ReadFromJsonAsync<FirebaseErrorResponseDto>();

                throw new BadRequestException(errorResponse.Error.Message);
            }

            var verificationResponseData = await verificationResponse.Content.ReadFromJsonAsync<FirebasePhoneVerificationResponseDto>();

            if (verificationResponseData is null) throw new BadRequestException(ErrorMessages.FirebaseCouldNotVerifyPhoneNumber);

            #endregion

            var phoneSignInRequestUri =
                new Uri($"{_options.IdentityToolkitBaseUrl}:signInWithPhoneNumber?key={_options.FirebaseApiKey}");

            // verification code is hardcoded for now -- see test users of project
            var phoneSignInRequestBody = new FirebasePhoneVerificationRequestDto
            {
                SessionInfo = verificationResponseData.SessionInfo,
                Code = 765432
            };

            var phoneSignInResponse = await _httpClient.PostAsJsonAsync(phoneSignInRequestUri, phoneSignInRequestBody);

            if (!phoneSignInResponse.IsSuccessStatusCode)
            {
                var errorResponse = await phoneSignInResponse.Content.ReadFromJsonAsync<FirebaseErrorResponseDto>();

                throw new BadRequestException(errorResponse.Error.Message);
            }

            var phoneSignInResponseData = await phoneSignInResponse.Content.ReadFromJsonAsync<FirebaseAuthResponseDto>();

            if (phoneSignInResponseData is null) throw new BadRequestException(ErrorMessages.FirebaseCouldNotSignInWithPhoneNumber);

            return phoneSignInResponseData;
        }

        public async Task<GoogleVerificationResponseDto> FirebaseGoogleLoginVerification(GoogleVerificationRequestDto googleVerificationRequestDto)
        {
            var requestUrl = new Uri($"{_options.IdentityToolkitBaseUrl}:signInWithIdp?key={_options.FirebaseApiKey}");

            var response = await _httpClient.PostAsJsonAsync(requestUrl, googleVerificationRequestDto);

            if (!response.IsSuccessStatusCode)
            {
                //TODO : handle unsuccessful case
            }

            var debugData = await response.Content.ReadAsStringAsync();

            var responseData = await response.Content.ReadFromJsonAsync<GoogleVerificationResponseDto>();

            return responseData;
        }
    }
}
