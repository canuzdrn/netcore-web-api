using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using userMS.Application.DTOs;
using userMS.Application.DTOs.Request;
using userMS.Application.Services;
using userMS.Infrastructure.Com;
using userMS.Infrastructure.Statics;

namespace userMS.API.Controllers
{
    [Route(RoutingUrls.BaseRoute)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppSettings _options;

        public AuthController(IAuthService authService,
            IOptions<AppSettings> options)
        {
            _authService = authService;
            _options = options.Value;
        }

        [HttpPost(RoutingUrls.Auth.Register)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseAuthResponseDto))]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var response = await _authService.RegisterUserAsync(registerDto);

            return Ok(response);
        }

        [HttpPost(RoutingUrls.Auth.LoginIdentifier)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseAuthResponseDto))]
        public async Task<IActionResult> LoginWithEmail([FromBody] UsernameOrEmailLoginUserDto loginDto)
        {
            var response = await _authService.IdentifierLoginUserAsync(loginDto);

            return Ok(response);
        }

        [HttpPost(RoutingUrls.Auth.LoginPhone)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseAuthResponseDto))]
        public async Task<IActionResult> LoginWithPhone([FromBody] PhoneLoginUserDto loginDto)
        {
            var response = await _authService.PhoneLoginUserAsync(loginDto);

            return Ok(response);
        }

        [HttpPost(RoutingUrls.Auth.SendEmailOtp)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendEmailOtp([FromBody] SendEmailOtpRequestDto sendEmailOtpRequestDto)
        {
            await _authService.SendEmailOtpAsync(sendEmailOtpRequestDto);

            return Ok();
        }

        [HttpPost(RoutingUrls.Auth.SendPhoneOtp)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendPhoneOtp([FromBody] SendPhoneOtpRequestDto sendPhoneOtpRequestDto)
        {
            await _authService.SendPhoneOtpAsync(sendPhoneOtpRequestDto);

            return Ok();
        }

        [HttpGet(RoutingUrls.Auth.VerifyEmailOtp)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        public async Task<IActionResult> VerifyEmailOtp([FromQuery] string transactionId)
        {
            var otpVerificationRequestDto = new OtpVerificationRequestDto
            {
                TransactionId = transactionId,
                Otp = transactionId.Substring(transactionId.Length - 6),
                VerificationMethod = Application.Enums.VerificationMethods.Email
            };
            var verificationResult = await _authService.VerifyOtpAsync(otpVerificationRequestDto);

            return Ok(verificationResult);
        }

        [HttpGet(RoutingUrls.Auth.VerifyPhoneOtp)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        public async Task<IActionResult> VerifyPhoneOtp([FromQuery] string transactionId)
        {
            var otpVerificationRequestDto = new OtpVerificationRequestDto
            {
                TransactionId = transactionId,
                Otp = transactionId.Substring(transactionId.Length - 6),
                VerificationMethod = Application.Enums.VerificationMethods.Phone
            };
            var verificationResult = await _authService.VerifyOtpAsync(otpVerificationRequestDto);

            return Ok(verificationResult);
        }

        [HttpGet(RoutingUrls.Auth.GoogleOauth)]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleLoginCallback)),
            };

            return Challenge(properties, "Google");
        }

        [HttpGet(RoutingUrls.Auth.GoogleOauthCallback)]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var authResult = await HttpContext.AuthenticateAsync("Google");

            if (!authResult.Succeeded)
            {
                // TODO : Handle authentication failure
                return BadRequest();
            }

            var providerId = "google.com";

            var accessToken = authResult.Properties.GetTokenValue("access_token");
            // TODO access token null handler

            var externalProviderOauthLoginRequestDto = new ExternalProviderOauthLoginRequestDto
            {
                AccessToken = accessToken,
                ProviderId = providerId
            };

            var verificationResult = await _authService.ExternalProviderOauthLogin(externalProviderOauthLoginRequestDto);

            return Ok(verificationResult);
        }

        [HttpGet(RoutingUrls.Auth.GithubOauth)]
        public IActionResult GithubLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GithubLoginCallback)),
            };

            return Challenge(properties, "Github");
        }

        [HttpGet(RoutingUrls.Auth.GithubOauthCallback)]
        public async Task<IActionResult> GithubLoginCallback()
        {
            var authResult = await HttpContext.AuthenticateAsync("Github");

            if (!authResult.Succeeded)
            {
                // TODO : Handle authentication failure
                return BadRequest();
            }

            var providerId = "github.com";

            var accessToken = authResult.Properties.GetTokenValue("access_token");

            var externalProviderOauthLoginRequestDto = new ExternalProviderOauthLoginRequestDto
            {
                AccessToken = accessToken,
                ProviderId = providerId
            };

            var verificationResult = await _authService.ExternalProviderOauthLogin(externalProviderOauthLoginRequestDto);

            return Ok(verificationResult);
        }

        //[HttpGet(RoutingUrls.Auth.MicrosoftOauth)]
        //public IActionResult MicrosoftLogin()
        //{
        //    var properties = new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Action(nameof(MicrosoftLoginCallback)),
        //    };

        //    return Challenge(properties, "Microsoft");
        //}

        //[HttpGet(RoutingUrls.Auth.MicrosoftOauthCallback)]
        //public async Task<IActionResult> MicrosoftLoginCallback()
        //{
        //    var authResult = await HttpContext.AuthenticateAsync("Microsoft");

        //    if (!authResult.Succeeded)
        //    {
        //        // TODO : Handle authentication failure
        //        return BadRequest();
        //    }

        //    var providerId = "microsoft.com";

        //    var accessToken = authResult.Properties.GetTokenValue("access_token");

        //    var externalProviderOauthLoginRequestDto = new ExternalProviderOauthLoginRequestDto
        //    {
        //        AccessToken = accessToken,
        //        ProviderId = providerId
        //    };

        //    var verificationResult = await _authService.ExternalProviderOauthLogin(externalProviderOauthLoginRequestDto);

        //    return Ok(verificationResult);
        //}
    }
}
