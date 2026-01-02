using PurchaseService.API.Models.DTOs;
using System.Text.Json;

namespace PurchaseService.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly string _accountServiceUrl;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _accountServiceUrl = configuration.GetValue<string>("AccountServiceBaseUrl") ?? "http://localhost:5001";
        }

        public async Task<LoginResponseDto> AuthenticateBuyerAsync(string email)
        {
            try
            {
                _logger.LogInformation("Authenticating buyer with email: {Email}", email);

                // Call Account Service to get user by email
                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/by-email/{email}");
                
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new LoginResponseDto
                        {
                            IsSuccess = false,
                            Message = "User not found. Please check your email address."
                        };
                    }
                    
                    _logger.LogWarning("Account service returned error: {StatusCode}", response.StatusCode);
                    return new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Authentication service temporarily unavailable. Please try again later."
                    };
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (user == null)
                {
                    return new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid user data received."
                    };
                }

                // Check if user is a buyer
                if (user.Role?.ToLower() != "buyer")
                {
                    return new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Access denied. This service is only available for buyers."
                    };
                }

                return new LoginResponseDto
                {
                    IsSuccess = true,
                    Message = "Authentication successful.",
                    UserId = user.Id,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating buyer with email: {Email}", email);
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred during authentication. Please try again."
                };
            }
        }

        public async Task<bool> ValidateBuyerAsync(Guid buyerId)
        {
            try
            {
                _logger.LogInformation("Validating buyer: {BuyerId}", buyerId);

                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/{buyerId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Buyer validation failed for ID: {BuyerId}. Status: {StatusCode}", 
                        buyerId, response.StatusCode);
                    return false;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<AccountUserDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                bool isValidBuyer = user != null && user.UserType == 2; // UserType 2 = Buyer
                
                if (!isValidBuyer)
                {
                    _logger.LogWarning("User {BuyerId} is not a valid buyer. UserType: {UserType}", buyerId, user?.UserType);
                }

                return isValidBuyer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating buyer: {BuyerId}", buyerId);
                return false;
            }
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/by-username/{username}");
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                return null;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/by-email/{email}");
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return null;
            }
        }

        public async Task<AuthenticatedUserDto?> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                var loginRequest = new { Email = email, Password = password };
                var jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_accountServiceUrl}/api/users/login", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var accountUser = JsonSerializer.Deserialize<AccountUserDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (accountUser == null)
                {
                    return null;
                }

                // Map UserType number to role string
                string role = accountUser.UserType switch
                {
                    1 => "seller",
                    2 => "buyer", 
                    3 => "carrier",
                    4 => "agent",
                    _ => "unknown"
                };

                return new AuthenticatedUserDto
                {
                    Id = accountUser.Id,
                    Username = accountUser.Email,
                    Email = accountUser.Email,
                    Role = role,
                    Token = "" // Not using tokens for now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user: {Email}", email);
                return null;
            }
        }
    }

    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AuthenticatedUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class AccountUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int UserType { get; set; }
        public int Status { get; set; }
    }
}