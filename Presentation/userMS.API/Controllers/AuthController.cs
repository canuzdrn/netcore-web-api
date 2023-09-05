using Microsoft.AspNetCore.Mvc;
using System.Net;
using userMS.Application.DTOs;
using userMS.Application.DTOs.Response;
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseRegisterResponseDto))]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await _authService.RegisterUserAsync(registerDto);

            var firebaseResponse = await _firebaseAuthService.FirebaseRegisterAsync(
                new FirebaseRequestDto
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                });

            // email
            await _emailService.SendRegisterEmailAsync(registerDto.Email);

            return Ok(firebaseResponse);
        }

        [HttpPost(RoutingUrls.Auth.Login)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FirebaseLoginResponseDto))]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var result = await _authService.LoginUserAsync(loginDto);

            var userEmail = await _authService.GetLoggedInEmailAsync(loginDto);

            var firebaseResponse = await _firebaseAuthService.FirebaseLoginAsync(
                new FirebaseRequestDto
                {
                    Email = userEmail,
                    Password = loginDto.Password,
                });

            // email
            await _emailService.SendLoginEmailAsync(userEmail);

            return Ok(firebaseResponse);
        }

    }
}
