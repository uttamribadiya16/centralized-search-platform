using PurchaseService.API.Models.DTOs;
using System.Text.Json;

namespace PurchaseService.API.Services
{
    public class OfferServiceClient : IOfferService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OfferServiceClient> _logger;

        public OfferServiceClient(HttpClient httpClient, ILogger<OfferServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OfferResponseDto?> GetOfferByIdAsync(Guid offerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/offers/{offerId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OfferResponseDto>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                
                _logger.LogWarning($"Offer not found: {offerId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting offer: {offerId}");
                return null;
            }
        }

        public async Task<PagedResult<OfferResponseDto>> GetAvailableOffersAsync(OfferSearchDto searchDto)
        {
            try
            {
                // Add status filter to only get available offers
                var queryParams = new List<string>
                {
                    $"page={searchDto.Page}",
                    $"pageSize={searchDto.PageSize}",
                    "status=Available"
                };

                if (!string.IsNullOrEmpty(searchDto.Make))
                    queryParams.Add($"make={Uri.EscapeDataString(searchDto.Make)}");

                if (!string.IsNullOrEmpty(searchDto.Model))
                    queryParams.Add($"model={Uri.EscapeDataString(searchDto.Model)}");

                if (searchDto.Year.HasValue)
                    queryParams.Add($"year={searchDto.Year}");

                if (searchDto.MinPrice.HasValue)
                    queryParams.Add($"minPrice={searchDto.MinPrice}");

                if (searchDto.MaxPrice.HasValue)
                    queryParams.Add($"maxPrice={searchDto.MaxPrice}");

                if (!string.IsNullOrEmpty(searchDto.Condition))
                    queryParams.Add($"condition={Uri.EscapeDataString(searchDto.Condition)}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"api/offers?{queryString}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PagedResult<OfferResponseDto>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new PagedResult<OfferResponseDto>();
                }

                _logger.LogWarning($"Failed to get offers: {response.StatusCode}");
                return new PagedResult<OfferResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available offers");
                return new PagedResult<OfferResponseDto>();
            }
        }
    }
}