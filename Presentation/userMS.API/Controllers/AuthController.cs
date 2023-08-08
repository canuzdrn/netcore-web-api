using Microsoft.AspNetCore.Mvc;
using userMS.Application.DTOs;
using userMS.Application.Services;

namespace userMS.API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("register")]
        // add response type
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await _authService.RegisterUserAsync(registerDto);

            // email
            await _emailService.SendRegisterEmailAsync(registerDto.Email);

            return Ok(result);
        }

        [HttpPost("login")]
        // add response type
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var result = await _authService.LoginUserAsync(loginDto);

            // email
            await _emailService.SendLoginEmailAsync(loginDto.Email);

            return Ok(result);
        }

    }
}
