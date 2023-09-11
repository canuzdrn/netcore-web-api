using Microsoft.AspNetCore.Mvc;
using System.Net;
using userMS.Application.DTOs;
using userMS.Application.DTOs.Request;
using userMS.Application.Services;
using userMS.Infrastructure.Statics;

namespace userMS.API.Controllers
{
    [Route(RoutingUrls.BaseRoute)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

        [HttpPost(RoutingUrls.Auth.VerifyOtp)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequestDto otpVerificationRequestDto)
        {
            var verificationResult = await _authService.VerifyOtpAsync(otpVerificationRequestDto);

            return Ok(verificationResult);
        }

    }
}
