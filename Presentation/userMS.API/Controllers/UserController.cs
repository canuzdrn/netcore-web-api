using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using userMS.Application.DTOs;
using userMS.Application.Services;
using userMS.Infrastructure.Statics;

namespace userMS.API.Controllers
{
    [Route(RoutingUrls.BaseRoute)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize] // this authorization attribute is added to test authentication with bearer token
        [HttpGet(RoutingUrls.User.GetAll)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserDto>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();   

            return Ok(users);
        }

        [HttpGet(RoutingUrls.User.GetById)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return Ok(user);
        }

        [HttpGet(RoutingUrls.User.GetByUsername)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);

            return Ok(user);
        }

        [HttpGet(RoutingUrls.User.GetByEmail)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUserByEmailAddress(string email)
        {
            var user = await _userService.GetUserByEmailAddressAsync(email);

            return Ok(user);
        }

        [HttpGet(RoutingUrls.User.GetByPhoneNo)]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserDto))]
        public async Task<IActionResult> GetUserByPhoneNumber(string phoneNo)
        {
            var user = await _userService.GetUserByPhoneNumberAsync(phoneNo);

            return Ok(user);
        }

        [HttpPost(RoutingUrls.User.Create)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var result = await _userService.AddUserAsync(userDto);

            return Created($"api/User/username/{result.UserName}",result);
        }

        [HttpPost(RoutingUrls.User.BulkCreate)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUsers([FromBody] IEnumerable<UserDto> userDtos)
        {
            var result = await _userService.AddUsersAsync(userDtos);

            return Ok(result);
        }

        [HttpPut(RoutingUrls.User.Update)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            var updateResult = await _userService.UpdateUserAsync(userDto);

            return Ok(updateResult);

        }

        [HttpPut(RoutingUrls.User.BulkUpdate)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUsers([FromBody] IEnumerable<UserDto> userDtos)
        {
            var updateResult = await _userService.UpdateUsersAsync(userDtos);

            return Ok(updateResult);
        }

        [HttpDelete(RoutingUrls.User.Delete)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteUser(UserDto userDto)
        {   
            await _userService.DeleteUserAsync(userDto);

            return NoContent();
        }

        [HttpDelete(RoutingUrls.User.DeleteById)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteUserById(Guid id)
        {
            await _userService.DeleteUserByIdAsync(id);

            return NoContent();
        }

        [HttpDelete(RoutingUrls.User.BulkDelete)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<UserDto> userDtos)
        {
            await _userService.DeleteUsersAsync(userDtos);

            return NoContent();
        }

    }
}
