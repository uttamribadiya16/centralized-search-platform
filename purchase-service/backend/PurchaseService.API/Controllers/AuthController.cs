using Microsoft.AspNetCore.Mvc;
using PurchaseService.API.Models.DTOs;
using PurchaseService.API.Services;

namespace PurchaseService.API.Controllers
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
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Email is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Password is required."
                    });
                }

                var authenticatedUser = await _authService.AuthenticateUserAsync(request.Email, request.Password);
                
                if (authenticatedUser == null)
                {
                    return Unauthorized(new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid email or password."
                    });
                }

                // Check if user is a buyer
                if (authenticatedUser.Role?.ToLower() != "buyer")
                {
                    return Unauthorized(new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Access denied. This service is only available for buyers."
                    });
                }

                var result = new LoginResponseDto
                {
                    IsSuccess = true,
                    Message = "Authentication successful.",
                    UserId = authenticatedUser.Id,
                    Email = authenticatedUser.Email,
                    Role = authenticatedUser.Role
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt for email: {Email}", request.Email);
                return StatusCode(500, new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred during login. Please try again."
                });
            }
        }

        [HttpGet("validate/{buyerId}")]
        public async Task<ActionResult<bool>> ValidateBuyer(Guid buyerId)
        {
            try
            {
                var isValid = await _authService.ValidateBuyerAsync(buyerId);
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating buyer: {BuyerId}", buyerId);
                return StatusCode(500, false);
            }
        }
    }

    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}