using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TransportService.API.Models.DTOs;
using TransportService.API.Services;

namespace TransportService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input data" });
                }

                var user = await _authService.AuthenticateUserAsync(loginRequest.Email, loginRequest.Password);
                
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Only allow carriers to login to transport service
                if (user.Role != "carrier")
                {
                    return Unauthorized(new { message = "Access denied. Only carriers can access this service." });
                }

                return Ok(new
                {
                    message = "Login successful",
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        username = user.Username,
                        role = user.Role ?? "carrier"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during carrier login for email: {Email}", loginRequest.Email);
                return StatusCode(500, new { message = "An error occurred during login. Please try again." });
            }
        }
    }
}