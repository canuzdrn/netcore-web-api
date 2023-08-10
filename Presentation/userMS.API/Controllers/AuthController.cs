using Microsoft.AspNetCore.Mvc;
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
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost(RoutingUrls.Auth.Register)]
        // add response type
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await _authService.RegisterUserAsync(registerDto);

            // email
            await _emailService.SendRegisterEmailAsync(registerDto.Email);

            return Ok(result);
        }

        [HttpPost(RoutingUrls.Auth.Login)]
        // add response type
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var result = await _authService.LoginUserAsync(loginDto);

            // email
            var userEmail = await _authService.GetLoggedInEmailAsync(loginDto);

            await _emailService.SendLoginEmailAsync(userEmail);

            return Ok(result);
        }

    }
}
