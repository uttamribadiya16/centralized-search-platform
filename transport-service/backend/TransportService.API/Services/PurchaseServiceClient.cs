using System.Text.Json;
using TransportService.API.Models.DTOs;

namespace TransportService.API.Services
{
    public class PurchaseServiceClient : IPurchaseServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PurchaseServiceClient> _logger;

        public PurchaseServiceClient(HttpClient httpClient, ILogger<PurchaseServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PurchaseDto?> GetPurchaseAsync(Guid purchaseId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/purchases/{purchaseId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get purchase {PurchaseId}: {StatusCode}", purchaseId, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PurchaseDto>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase {PurchaseId}", purchaseId);
                return null;
            }
        }

        public async Task<PagedResult<PurchaseDto>> GetPurchasesAsync(Dictionary<string, object?> searchParams)
        {
            try
            {
                var queryParams = new List<string>();

                foreach (var param in searchParams)
                {
                    if (param.Value != null)
                    {
                        if (param.Value is DateTime dateValue)
                        {
                            queryParams.Add($"{param.Key}={dateValue:yyyy-MM-ddTHH:mm:ss.fffZ}");
                        }
                        else
                        {
                            queryParams.Add($"{param.Key}={Uri.EscapeDataString(param.Value.ToString()!)}");
                        }
                    }
                }

                var queryString = string.Join("&", queryParams);
                var url = string.IsNullOrEmpty(queryString) ? "/api/purchases" : $"/api/purchases?{queryString}";

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get purchases: {StatusCode}", response.StatusCode);
                    return new PagedResult<PurchaseDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PagedResult<PurchaseDto>>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return result ?? new PagedResult<PurchaseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases");
                return new PagedResult<PurchaseDto>();
            }
        }

    }
}