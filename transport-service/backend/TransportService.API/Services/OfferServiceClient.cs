using System.Text.Json;
using TransportService.API.Models.DTOs;

namespace TransportService.API.Services
{
    public class OfferServiceClient : IOfferServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OfferServiceClient> _logger;

        public OfferServiceClient(HttpClient httpClient, ILogger<OfferServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OfferDto?> GetOfferAsync(Guid offerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/offers/{offerId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get offer {OfferId}: {StatusCode}", offerId, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OfferDto>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offer {OfferId}", offerId);
                return null;
            }
        }

        public async Task<PagedResult<OfferDto>> GetOffersAsync(Dictionary<string, object?> searchParams)
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
                var url = string.IsNullOrEmpty(queryString) ? "/api/offers" : $"/api/offers?{queryString}";

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get offers: {StatusCode}", response.StatusCode);
                    return new PagedResult<OfferDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PagedResult<OfferDto>>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return result ?? new PagedResult<OfferDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offers");
                return new PagedResult<OfferDto>();
            }
        }

    }
}