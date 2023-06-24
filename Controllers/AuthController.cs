using Microsoft.AspNetCore.Mvc;
using ChatGptApi.Models;
using ChatGptApi.Services;
using Microsoft.Extensions.Configuration;

namespace ChatGptApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration, AuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            User user = await _authService.Register(userDto);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto userDto)
        {
            string token = await _authService.Login(userDto);

            if (token == "User not found.")
            {
                return BadRequest("Invalid credentials.");
            }

            if (token == "Wrong password.")
            {
                return BadRequest("Invalid credentials.");
            }

            return Ok(token);
        }

        [HttpGet("getUsers")]
        public async Task<List<User>> Get() => await _authService.GetAsync();

        [HttpPost("createUser")]
        public async Task<IActionResult> Post(User newUser)
        {
            await _authService.CreateAsync(newUser);
            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        }
    }
}
