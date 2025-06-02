using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoaSystems.Libraries.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoaSystems.WebAPIDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure this controller requires authorization
    public class SecurityController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public SecurityController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // POST: api/Auth/Login
        [HttpPost("HardCodedLogin")]
        [AllowAnonymous] // Allow anonymous access for login endpoint
        public IActionResult HardCodedLogin([FromBody] UsrCredentials login)
        {
            // Hardcoded login for demo purposes
            if (login.Username == "admin" && login.Password == "password123")
            {
                var token = _tokenService.GenerateToken(login.Username);
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid username or password");
        }
    }
    public class UsrCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
