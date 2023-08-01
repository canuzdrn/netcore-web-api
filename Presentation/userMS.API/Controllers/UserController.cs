using Microsoft.AspNetCore.Mvc;
using userMS.Application.Services;
using userMS.Domain.Entities;

namespace userMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users);
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(user);
        }

        [HttpGet("username/{username}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByEmailAddress(string email)
        {
            var user = await _userService.GetUserByEmailAddressAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(user);
        }

        [HttpGet("phoneNo/{phoneNo}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByPhoneNumber(string phoneNo)
        {
            var user = await _userService.GetUserByPhoneNumberAsync(phoneNo);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(user);
        }

        [HttpPost("add/one")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var result = await _userService.AddUserAsync(user);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost("add/batch")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUsers([FromBody] IEnumerable<User> users)
        {
            return Ok(await _userService.AddUsersAsync(users));
        }

        [HttpPut("update/one")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var updateResult = await _userService.UpdateUserAsync(user);

            if (updateResult == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(updateResult);

        }

        [HttpPut("update/batch")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUsers([FromBody] IEnumerable<User> users)
        {
            var updateResult = await _userService.UpdateUsersAsync(users);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(updateResult);
        }

        [HttpDelete("delete/one")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(User user)
        {
            var deleteResult = await _userService.DeleteUserAsync(user);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("delete/one/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUserById(string id)
        {
            var deleteResult = await _userService.DeleteUserByIdAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("delete/batch")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<User> users)
        {
            var deleteResult = await _userService.DeleteUsersAsync(users);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
