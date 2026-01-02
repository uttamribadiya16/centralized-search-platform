using Microsoft.AspNetCore.Mvc;
using OfferService.API.Services;

namespace OfferService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                // Call Account Service login endpoint to authenticate
                var loginResult = await _authService.AuthenticateUserAsync(request.Email, request.Password);
                
                if (loginResult == null)
                {
                    _logger.LogWarning("Login failed: Authentication failed for email {Email}", request.Email);
                    return BadRequest(new { message = "Invalid email or password" });
                }

                // Verify user is a seller
                if (loginResult.UserType != "Seller")
                {
                    _logger.LogWarning("Login failed: User is not a seller - {Email}, UserType: {UserType}", 
                        request.Email, loginResult.UserType);
                    return BadRequest(new { message = "Only sellers can access the offer service" });
                }

                _logger.LogInformation("Login successful for seller: {Email}", request.Email);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    User = new UserDto
                    {
                        Id = loginResult.Id,
                        Username = loginResult.Email,
                        Email = loginResult.Email,
                        FirstName = loginResult.FirstName,
                        LastName = loginResult.LastName,
                        UserType = loginResult.UserType
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during login", details = ex.Message });
            }
        }

        [HttpPost("validate-seller")]
        public async Task<ActionResult> ValidateSeller([FromBody] ValidateSellerRequest request)
        {
            try
            {
                var isValid = await _authService.ValidateSellerAsync(request.SellerId);
                return Ok(new { isValid, message = isValid ? "Valid seller" : "Invalid seller" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating seller {SellerId}", request.SellerId);
                return StatusCode(500, new { message = "An error occurred while validating seller" });
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }

    public class ValidateSellerRequest
    {
        public Guid SellerId { get; set; }
    }
}