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

        // [HttpPost("register")]
        // public ActionResult<User> Register(UserDto userDto)
        // {
        //     // AuthService authService = new AuthService();
        //     User user = _authService.Register(userDto);
        //     return Ok(user);
        // }

        // [HttpPost("login")]
        // public ActionResult<string> Login(UserDto userDto)
        // {
        //     // AuthService authService = new AuthService();
        //     string token = _authService.Login(userDto);
        //     if (token == null)
        //     {
        //         return BadRequest("Invalid credentials.");
        //     }

        //     return Ok(token);
        // }

        [HttpGet]
        public async Task<List<User>> Get() => await _authService.GetAsync();

        [HttpPost]
        public async Task<IActionResult> Post(User newUser)
        {
            await _authService.CreateAsync(newUser);
            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        }

        // [HttpPut("{id:length(24)}")]
        // public async Task<IActionResult> Update(string id, User updatedUser)
        // {
        //     var user = await _authService.GetAsync(id);

        //     if (user is null)
        //     {
        //         return NotFound();
        //     }

        //     updatedUser.Id = user.Id;

        //     await _authService.UpdateAsync(id, updatedUser);

        //     return NoContent();
        // }
    }
}
