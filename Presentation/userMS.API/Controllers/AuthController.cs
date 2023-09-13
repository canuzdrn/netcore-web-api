using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using userMS.Application.DTOs;
using userMS.Application.DTOs.Request;
using userMS.Application.DTOs.Response;
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
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly AppSettings _options;

        public AuthController(IAuthService authService,
            IFirebaseAuthService firebaseAuthService,
            IOptions<AppSettings> options)
        {
            _authService = authService;
            _firebaseAuthService = firebaseAuthService;
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

        // endpoints that serves user to google login flow
        [HttpGet("account/google-login")]
        public async Task<IActionResult> GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleLoginCallback)),
            };

            return Challenge(properties, "Google");
        }

        // endpoints that redirects user to google login page
        [HttpGet("account/google-login-callback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var authResult = await HttpContext.AuthenticateAsync("Google");

            if (!authResult.Succeeded)
            {
                // TODO : Handle authentication failure
                return BadRequest();
            }

            var accessToken = authResult.Properties.GetTokenValue("access_token");

            return Ok(accessToken);
        }

        [HttpPost("account/google-login-to-firebase")]
        public async Task<IActionResult> GoogleLoginToFirebase([FromBody] GoogleVerificationRequestDto googleVerificationRequestDto)
        {
            var verificationResult = await _firebaseAuthService.FirebaseGoogleLoginVerification(googleVerificationRequestDto);

            return Ok(verificationResult);
        }

    }
}
