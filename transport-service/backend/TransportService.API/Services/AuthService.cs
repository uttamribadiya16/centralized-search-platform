using TransportService.API.Models.DTOs;
using System.Text.Json;

namespace TransportService.API.Services
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

        public async Task<bool> ValidateCarrierAsync(Guid carrierId)
        {
            try
            {
                _logger.LogInformation("Validating carrier: {CarrierId}", carrierId);

                var response = await _httpClient.GetAsync($"{_accountServiceUrl}/api/users/{carrierId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Carrier validation failed for ID: {CarrierId}. Status: {StatusCode}", 
                        carrierId, response.StatusCode);
                    return false;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<AccountUserDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                bool isValidCarrier = user != null && user.UserType == 3; // UserType 3 = Carrier
                
                if (!isValidCarrier)
                {
                    _logger.LogWarning("User {CarrierId} is not a valid carrier. UserType: {UserType}", carrierId, user?.UserType);
                }

                return isValidCarrier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating carrier: {CarrierId}", carrierId);
                return false;
            }
        }
    }
}