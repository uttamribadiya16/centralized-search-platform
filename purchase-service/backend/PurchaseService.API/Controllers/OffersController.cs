using Microsoft.AspNetCore.Mvc;
using PurchaseService.API.Models.DTOs;
using PurchaseService.API.Services;

namespace PurchaseService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;
        private readonly IAuthService _authService;
        private readonly ILogger<OffersController> _logger;

        public OffersController(
            IOfferService offerService,
            IAuthService authService,
            ILogger<OffersController> logger)
        {
            _offerService = offerService;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<OfferResponseDto>>> GetAvailableOffers([FromQuery] OfferSearchDto searchDto)
        {
            try
            {
                var result = await _offerService.GetAvailableOffersAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available offers");
                return StatusCode(500, new { message = "An error occurred while retrieving offers" });
            }
        }

        [HttpGet("{offerId}")]
        public async Task<ActionResult<OfferResponseDto>> GetOffer(Guid offerId)
        {
            try
            {
                var offer = await _offerService.GetOfferByIdAsync(offerId);
                if (offer == null)
                {
                    return NotFound(new { message = "Offer not found" });
                }

                return Ok(offer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offer {OfferId}", offerId);
                return StatusCode(500, new { message = "An error occurred while retrieving the offer" });
            }
        }
    }
}