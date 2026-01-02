using OfferService.API.Models.DTOs;
using System.Text.Json;

namespace OfferService.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> AuthenticateSellerAsync(string email);
        Task<bool> ValidateSellerAsync(Guid sellerId);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<AuthenticatedUserDto?> AuthenticateUserAsync(string email, string password);
    }

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

        public async Task<LoginResponseDto> AuthenticateSellerAsync(string email)
        {
            try
            {
                _logger.LogInformation("Authenticating seller with email: {Email}", email);

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
                        Message = "Authentication service unavailable. Please try again later."
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<AccountUserDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (userResponse == null)
                {
                    return new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid response from authentication service."
                    };
                }

                // Check if user is a seller (UserType = 1)
                if (userResponse.UserType != 1)
                {
                    return new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid user. Only sellers can access this system."
                    };
                }

                // Check if user is active
                if (userResponse.Status != 1)
                {
                    return new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Your account is not active. Please contact support."
                    };
                }

                // Generate a simple token (in production, use proper JWT)
                var token = GenerateToken(userResponse.Id);

                return new LoginResponseDto
                {
                    IsSuccess = true,
                    UserId = userResponse.Id,
                    FullName = userResponse.FullName,
                    Email = userResponse.Email,
                    Token = token,
                    Message = "Authentication successful"
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while calling Account Service for email: {Email}", email);
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Network error. Please check your connection and try again."
                };
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while calling Account Service for email: {Email}", email);
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Request timeout. Please try again."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during authentication for email: {Email}", email);
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred. Please try again later."
                };
            }
        }

        private string GenerateToken(Guid userId)
        {
            // In production, use proper JWT token generation
            // For now, we'll use a simple base64 encoded string
            var tokenData = $"{userId}:{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}";
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        public async Task<bool> ValidateSellerAsync(Guid sellerId)
        {
            try
            {
                _logger.LogInformation("Validating seller with ID: {SellerId}", sellerId);

                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/{sellerId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to validate seller {SellerId}: {StatusCode}", sellerId, response.StatusCode);
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<AccountUserDto>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (user == null)
                {
                    _logger.LogWarning("User data is null for seller {SellerId}", sellerId);
                    return false;
                }

                var isSeller = user.UserType == 1; // 1 = Seller
                _logger.LogInformation("Seller validation result for {SellerId}: {IsSeller}, UserType: {UserType}", 
                    sellerId, isSeller, user.UserType);

                return isSeller;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating seller {SellerId}", sellerId);
                return false;
            }
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            try
            {
                _logger.LogInformation("Getting user by username: {Username}", username);

                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/username/{username}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get user {Username}: {StatusCode}", username, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<AccountUserDto>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (user == null)
                {
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user {Username}", username);
                return new UserDto
                {
                    Id = user.Id,
                    Username = username,
                    Email = user.Email,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType == 1 ? "Seller" : "Other"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username {Username}", username);
                return null;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Getting user by email: {Email}", email);

                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/by-email/{email}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get user {Email}: {StatusCode}", email, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<AccountUserDto>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (user == null)
                {
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user {Email}", email);
                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Email, // Use email as username for compatibility
                    Email = user.Email,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType == 1 ? "Seller" : "Other"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email {Email}", email);
                return null;
            }
        }
        public async Task<AuthenticatedUserDto?> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation("Authenticating user with email: {Email}", email);

                // Call Account Service login endpoint
                var loginRequest = new
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(loginRequest, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_accountServiceUrl}/api/users/login", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to authenticate user {Email}: {StatusCode}", email, response.StatusCode);
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<AccountUserDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (user == null)
                {
                    return null;
                }

                _logger.LogInformation("Successfully authenticated user {Email}", email);
                return new AuthenticatedUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType == 1 ? "Seller" : user.UserType == 2 ? "Buyer" : user.UserType == 3 ? "Carrier" : "Agent"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user {Email}", email);
                return null;
            }
        }
    }

    // DTO for authenticated user response
    public class AuthenticatedUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }

    // DTO for Account Service response
    public class AccountUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int UserType { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // DTO for user data
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }
    
}