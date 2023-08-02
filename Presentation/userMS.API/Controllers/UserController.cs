﻿using Microsoft.AspNetCore.Mvc;
using userMS.Application.DTOs;
using userMS.Application.Services;

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
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();   

            return Ok(users);
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("username/{username}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByEmailAddress(string email)
        {
            var user = await _userService.GetUserByEmailAddressAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("phoneNo/{phoneNo}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByPhoneNumber(string phoneNo)
        {
            var user = await _userService.GetUserByPhoneNumberAsync(phoneNo);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("add/one")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            var result = await _userService.AddUserAsync(userDto);

            return Ok(result);
        }

        [HttpPost("add/batch")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateUsers([FromBody] IEnumerable<UserDto> userDtos)
        {
            var result = await _userService.AddUsersAsync(userDtos);

            return Ok(result);
        }

        [HttpPut("update/one")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            var updateResult = await _userService.UpdateUserAsync(userDto);

            if (updateResult == null)
            {
                return NotFound();
            }

            return Ok(updateResult);

        }

        [HttpPut("update/batch")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateUsers([FromBody] IEnumerable<UserDto> userDtos)
        {
            var updateResult = await _userService.UpdateUsersAsync(userDtos);

            return Ok(updateResult);
        }

        [HttpDelete("delete/one")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(UserDto userDto)
        {   
            var deleteResult = await _userService.DeleteUserAsync(userDto);

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
        public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<UserDto> userDtos)
        {
            var deleteResult = await _userService.DeleteUsersAsync(userDtos);

            if (!deleteResult)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}