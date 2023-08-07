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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        // add response type
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await _authService.RegisterUserAsync(registerDto);

            return Ok(result);
        }

        [HttpPost("login")]
        // add response type
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var result = await _authService.LoginUserAsync(loginDto);

            return Ok(result);
        }

    }
}
