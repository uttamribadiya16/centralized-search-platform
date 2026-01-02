using Microsoft.AspNetCore.Mvc;
using SearchService.API.Models;
using SearchService.API.Services;

namespace SearchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IElasticsearchService elasticsearchService, ILogger<SearchController> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    /// <summary>
    /// Search offers for a specific seller
    /// </summary>
    /// <param name="sellerId">The seller's unique identifier</param>
    /// <param name="searchText">Optional search text to filter results</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <returns>Search results containing the seller's offers</returns>
    [HttpGet("offers")]
    public async Task<ActionResult<SearchResponse>> SearchOffers(
        [FromQuery] Guid sellerId,
        [FromQuery] string searchText = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Search request for seller {SellerId} with text: {SearchText}", sellerId, searchText);

            // Validate parameters
            if (sellerId == Guid.Empty)
            {
                _logger.LogWarning("Invalid seller ID provided: {SellerId}", sellerId);
                return BadRequest("Valid seller ID is required");
            }

            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1 || pageSize > 100)
            {
                pageSize = 20;
            }

            var request = new SearchRequest
            {
                SellerId = sellerId,
                SearchText = searchText?.Trim() ?? string.Empty,
                Page = page,
                PageSize = pageSize
            };

            var response = await _elasticsearchService.SearchOffersAsync(request);

            _logger.LogInformation("Search completed for seller {SellerId}. Found {TotalCount} results", 
                sellerId, response.TotalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search for seller {SellerId}", sellerId);
            return StatusCode(500, "An error occurred while searching offers");
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public async Task<ActionResult> HealthCheck()
    {
        try
        {
            // Simple health check - try to initialize the index (won't recreate if exists)
            var isHealthy = await _elasticsearchService.InitializeIndexAsync();
            
            if (isHealthy)
            {
                return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
            }
            else
            {
                return StatusCode(503, new { status = "unhealthy", timestamp = DateTime.UtcNow });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new { status = "unhealthy", error = ex.Message, timestamp = DateTime.UtcNow });
        }
    }
}