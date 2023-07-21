using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using userMS.Models;
using userMS.Repository;

namespace userMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGenericRepository<User> _genericRepository;

        public UserController(IGenericRepository<User> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllUsers() {
            var users = _genericRepository.GetAll();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public IActionResult CreateUser([FromBody] User user)
        { 

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_genericRepository.Create(user));
        }
    }
}
