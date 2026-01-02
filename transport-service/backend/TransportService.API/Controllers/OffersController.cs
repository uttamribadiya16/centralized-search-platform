using Microsoft.AspNetCore.Mvc;
using TransportService.API.Services;

namespace TransportService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IOfferServiceClient _offerService;
        private readonly ILogger<OffersController> _logger;

        public OffersController(IOfferServiceClient offerService, ILogger<OffersController> logger)
        {
            _offerService = offerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOffers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? make = null,
            [FromQuery] string? model = null,
            [FromQuery] int? year = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? condition = null,
            [FromQuery] string? location = null)
        {
            try
            {
                var searchParams = new Dictionary<string, object?>
                {
                    ["page"] = page,
                    ["pageSize"] = pageSize,
                    ["make"] = make,
                    ["model"] = model,
                    ["year"] = year,
                    ["minPrice"] = minPrice,
                    ["maxPrice"] = maxPrice,
                    ["condition"] = condition,
                    ["location"] = location
                };

                var result = await _offerService.GetOffersAsync(searchParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offers with search parameters");
                return StatusCode(500, new { message = "An error occurred while retrieving offers" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOffer(Guid id)
        {
            try
            {
                var offer = await _offerService.GetOfferAsync(id);
                
                if (offer == null)
                {
                    return NotFound(new { message = "Offer not found" });
                }

                return Ok(offer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offer {OfferId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the offer" });
            }
        }
    }
}