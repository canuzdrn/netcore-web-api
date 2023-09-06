using Microsoft.AspNetCore.Mvc;
using System.Net;
using userMS.Application.DTOs;
using userMS.Application.Services;
using userMS.Infrastructure.Statics;

namespace userMS.API.Controllers
{
    [Route(RoutingUrls.BaseRoute)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService,
            IFirebaseAuthService firebaseAuthService,
            IEmailService emailService)
        {
            _authService = authService;
            _firebaseAuthService = firebaseAuthService;
            _emailService = emailService;
        }

        [HttpPost(RoutingUrls.Auth.Register)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseAuthResponseDto))]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await _authService.RegisterUserAsync(registerDto);

            var firebaseResponse = await _firebaseAuthService.FirebaseRegisterAsync(
                new FirebaseEmailSignInRequestDto
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                });

            // email
            await _emailService.SendRegisterEmailAsync(registerDto.Email);

            return Ok(firebaseResponse);
        }

        [HttpPost(RoutingUrls.Auth.LoginIdentifier)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseAuthResponseDto))]
        public async Task<IActionResult> LoginWithEmail([FromBody] UsernameOrEmailLoginUserDto loginDto)
        {
            var result = await _authService.IdentifierLoginUserAsync(loginDto);

            var userEmail = await _authService.GetLoggedInEmailAsync(loginDto);

            var firebaseResponse = await _firebaseAuthService.FirebaseEmailLoginAsync(
                new FirebaseEmailSignInRequestDto
                {
                    Email = userEmail,
                    Password = loginDto.Password,
                });

            // email
            await _emailService.SendLoginEmailAsync(userEmail);

            return Ok(firebaseResponse);
        }

        [HttpPost(RoutingUrls.Auth.LoginPhone)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseAuthResponseDto))]
        public async Task<IActionResult> LoginWithPhone([FromBody] PhoneLoginUserDto loginDto)
        {
            var result = await _authService.PhoneLoginUserAsync(loginDto);

            var firebaseResponse = await _firebaseAuthService.FirebasePhoneLoginAsync(
                new FirebasePhoneSignInRequestDto
                {
                    PhoneNumber = loginDto.PhoneNumber,
                });

            // email
            var userEmail = await _authService.GetLoggedInEmailAsync(loginDto);
            await _emailService.SendLoginEmailAsync(userEmail);

            return Ok(firebaseResponse);
        }

    }
}
