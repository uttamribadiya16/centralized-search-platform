using Microsoft.AspNetCore.Mvc;
using OfferService.API.Models.DTOs;
using OfferService.API.Services;

namespace OfferService.API.Controllers
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
        public async Task<ActionResult<PagedResult<OfferResponseDto>>> GetOffers([FromQuery] OfferSearchDto searchDto)
        {
            try
            {
                var result = await _offerService.GetOffersAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offers");
                return StatusCode(500, new { message = "An error occurred while retrieving offers" });
            }
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<PagedResult<OfferResponseDto>>> GetOffersBySeller(
            Guid sellerId, 
            [FromQuery] OfferSearchDto searchDto)
        {
            try
            {
                // Verify seller exists
                var sellerExists = await _authService.ValidateSellerAsync(sellerId);
                if (!sellerExists)
                {
                    return NotFound(new { message = "Seller not found" });
                }

                var result = await _offerService.GetOffersBySellerAsync(sellerId, searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offers for seller {SellerId}", sellerId);
                return StatusCode(500, new { message = "An error occurred while retrieving offers" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OfferResponseDto>> GetOffer(Guid id)
        {
            try
            {
                var offer = await _offerService.GetOfferByIdAsync(id);
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

        [HttpPost]
        public async Task<ActionResult<OfferResponseDto>> CreateOffer(CreateOfferDto createOfferDto)
        {
            try
            {
                _logger.LogInformation("Creating new offer for seller {SellerId}", createOfferDto.SellerId);

                // Validate seller
                var sellerExists = await _authService.ValidateSellerAsync(createOfferDto.SellerId);
                if (!sellerExists)
                {
                    return BadRequest(new { message = "Invalid seller. Only registered sellers can create offers." });
                }

                // Check if VIN already exists (only if VIN is provided)
                if (!string.IsNullOrEmpty(createOfferDto.VIN))
                {
                    var vinExists = await _offerService.VINExistsAsync(createOfferDto.VIN);
                    if (vinExists)
                    {
                        return BadRequest(new { message = "A vehicle with this VIN already exists." });
                    }
                }

                var result = await _offerService.CreateOfferAsync(createOfferDto);
                return CreatedAtAction(nameof(GetOffer), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating offer");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating offer for seller {SellerId}", createOfferDto.SellerId);
                return StatusCode(500, new { message = "An error occurred while creating the offer", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OfferResponseDto>> UpdateOffer(Guid id, UpdateOfferDto updateOfferDto)
        {
            try
            {
                _logger.LogInformation("Updating offer {OfferId}", id);

                // Get existing offer to verify ownership
                var existingOffer = await _offerService.GetOfferByIdAsync(id);
                if (existingOffer == null)
                {
                    return NotFound(new { message = "Offer not found" });
                }

                // Validate seller ownership (optional: you could add seller ID to the update DTO)
                var sellerExists = await _authService.ValidateSellerAsync(existingOffer.SellerId);
                if (!sellerExists)
                {
                    return BadRequest(new { message = "Invalid seller" });
                }

                var result = await _offerService.UpdateOfferAsync(id, updateOfferDto);
                if (result == null)
                {
                    return NotFound(new { message = "Offer not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating offer {OfferId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the offer", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOffer(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting offer {OfferId}", id);

                // Get existing offer to verify ownership
                var existingOffer = await _offerService.GetOfferByIdAsync(id);
                if (existingOffer == null)
                {
                    return NotFound(new { message = "Offer not found" });
                }

                // Validate seller ownership
                var sellerExists = await _authService.ValidateSellerAsync(existingOffer.SellerId);
                if (!sellerExists)
                {
                    return BadRequest(new { message = "Invalid seller" });
                }

                var result = await _offerService.DeleteOfferAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Offer not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting offer {OfferId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the offer", details = ex.Message });
            }
        }

        [HttpGet("featured")]
        public async Task<ActionResult<List<OfferResponseDto>>> GetFeaturedOffers()
        {
            try
            {
                var offers = await _offerService.GetFeaturedOffersAsync();
                return Ok(offers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured offers");
                return StatusCode(500, new { message = "An error occurred while retrieving featured offers" });
            }
        }

        [HttpGet("seller/{sellerId}/stats")]
        public async Task<ActionResult<Dictionary<string, object>>> GetSellerStats(Guid sellerId)
        {
            try
            {
                // Validate seller
                var sellerExists = await _authService.ValidateSellerAsync(sellerId);
                if (!sellerExists)
                {
                    return NotFound(new { message = "Seller not found" });
                }

                var stats = await _offerService.GetSellerStatsAsync(sellerId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stats for seller {SellerId}", sellerId);
                return StatusCode(500, new { message = "An error occurred while retrieving seller stats" });
            }
        }

        [HttpPost("validate-vin")]
        public async Task<ActionResult> ValidateVIN([FromBody] VINValidationDto request)
        {
            try
            {
                var exists = await _offerService.VINExistsAsync(request.VIN, request.ExcludeOfferId);
                return Ok(new { exists, message = exists ? "VIN already exists" : "VIN is available" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating VIN {VIN}", request.VIN);
                return StatusCode(500, new { message = "An error occurred while validating the VIN" });
            }
        }
    }

    public class VINValidationDto
    {
        public string VIN { get; set; } = string.Empty;
        public Guid? ExcludeOfferId { get; set; }
    }
}