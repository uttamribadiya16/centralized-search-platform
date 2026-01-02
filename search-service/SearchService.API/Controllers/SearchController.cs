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
    /// Search purchases for a specific buyer
    /// </summary>
    /// <param name="buyerId">The buyer's unique identifier</param>
    /// <param name="searchText">Optional search text to filter results</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <returns>Search results containing the buyer's purchases</returns>
    [HttpGet("purchases")]
    public async Task<ActionResult<SearchResponse>> SearchPurchases(
        [FromQuery] Guid buyerId,
        [FromQuery] string searchText = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Search request for buyer {BuyerId} with text: {SearchText}", buyerId, searchText);

            // Validate parameters
            if (buyerId == Guid.Empty)
            {
                _logger.LogWarning("Invalid buyer ID provided: {BuyerId}", buyerId);
                return BadRequest("Valid buyer ID is required");
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
                BuyerId = buyerId,
                SearchText = searchText?.Trim() ?? string.Empty,
                Page = page,
                PageSize = pageSize
            };

            var response = await _elasticsearchService.SearchPurchasesAsync(request);

            _logger.LogInformation("Search completed for buyer {BuyerId}. Found {TotalCount} results", 
                buyerId, response.TotalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search for buyer {BuyerId}", buyerId);
            return StatusCode(500, "An error occurred while searching purchases");
        }
    }

    /// <summary>
    /// Search transports for a specific carrier
    /// </summary>
    /// <param name="carrierId">The carrier's unique identifier</param>
    /// <param name="searchText">Optional search text to filter results</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <returns>Search results containing the carrier's transports</returns>
    [HttpGet("transports")]
    public async Task<ActionResult<SearchResponse>> SearchTransports(
        [FromQuery] Guid carrierId,
        [FromQuery] string searchText = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Search request for carrier {CarrierId} with text: {SearchText}", carrierId, searchText);

            // Validate parameters
            if (carrierId == Guid.Empty)
            {
                _logger.LogWarning("Invalid carrier ID provided: {CarrierId}", carrierId);
                return BadRequest("Valid carrier ID is required");
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
                CarrierId = carrierId,
                SearchText = searchText?.Trim() ?? string.Empty,
                Page = page,
                PageSize = pageSize
            };

            var response = await _elasticsearchService.SearchTransportsAsync(request);

            _logger.LogInformation("Search completed for carrier {CarrierId}. Found {TotalCount} results", 
                carrierId, response.TotalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search for carrier {CarrierId}", carrierId);
            return StatusCode(500, "An error occurred while searching transports");
        }
    }

    /// <summary>
    /// Search all types of data
    /// </summary>
    /// <param name="sellerId">Optional seller ID for offers</param>
    /// <param name="buyerId">Optional buyer ID for purchases</param>
    /// <param name="carrierId">Optional carrier ID for transports</param>
    /// <param name="searchText">Optional search text to filter results</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <returns>Search results containing offers, purchases, and transports</returns>
    [HttpGet("all")]
    public async Task<ActionResult<SearchResponse>> SearchAll(
        [FromQuery] Guid? sellerId,
        [FromQuery] Guid? buyerId,
        [FromQuery] Guid? carrierId,
        [FromQuery] string searchText = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Search all request with text: {SearchText}", searchText);

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
                SellerId = sellerId ?? Guid.Empty,
                BuyerId = buyerId ?? Guid.Empty,
                CarrierId = carrierId ?? Guid.Empty,
                SearchText = searchText?.Trim() ?? string.Empty,
                Page = page,
                PageSize = pageSize
            };

            var response = await _elasticsearchService.SearchAllAsync(request);

            _logger.LogInformation("Search all completed. Found {TotalCount} results", response.TotalCount);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search all");
            return StatusCode(500, "An error occurred while searching all data");
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