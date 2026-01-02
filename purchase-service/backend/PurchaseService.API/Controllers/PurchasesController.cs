using Microsoft.AspNetCore.Mvc;
using PurchaseService.API.Models.DTOs;
using PurchaseService.API.Services;

namespace PurchaseService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IAuthService _authService;
        private readonly ILogger<PurchasesController> _logger;

        public PurchasesController(
            IPurchaseService purchaseService,
            IAuthService authService,
            ILogger<PurchasesController> logger)
        {
            _purchaseService = purchaseService;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PurchaseResponseDto>>> GetPurchases([FromQuery] PurchaseSearchDto searchDto)
        {
            try
            {
                var result = await _purchaseService.GetPurchasesAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases");
                return StatusCode(500, new { message = "An error occurred while retrieving purchases" });
            }
        }

        [HttpGet("buyer/{buyerId}")]
        public async Task<ActionResult<PagedResult<PurchaseResponseDto>>> GetPurchasesByBuyer(
            Guid buyerId,
            [FromQuery] PurchaseSearchDto searchDto)
        {
            try
            {
                // Verify buyer exists and is valid
                var buyerExists = await _authService.ValidateBuyerAsync(buyerId);
                if (!buyerExists)
                {
                    return NotFound(new { message = "Buyer not found or not valid" });
                }

                var result = await _purchaseService.GetPurchasesByBuyerAsync(buyerId, searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases for buyer {BuyerId}", buyerId);
                return StatusCode(500, new { message = "An error occurred while retrieving purchases" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseResponseDto>> GetPurchase(Guid id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    return NotFound(new { message = "Purchase not found" });
                }

                return Ok(purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase {PurchaseId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the purchase" });
            }
        }

        [HttpPost("buyer/{buyerId}")]
        public async Task<ActionResult<PurchaseResponseDto>> CreatePurchase(
            Guid buyerId,
            [FromBody] PurchaseCreateDto createDto)
        {
            try
            {
                // Verify buyer exists and is valid
                var buyerExists = await _authService.ValidateBuyerAsync(buyerId);
                if (!buyerExists)
                {
                    return NotFound(new { message = "Buyer not found or not valid" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var purchase = await _purchaseService.CreatePurchaseAsync(buyerId, createDto);
                return CreatedAtAction(
                    nameof(GetPurchase),
                    new { id = purchase.Id },
                    purchase);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid purchase creation request for buyer {BuyerId}", buyerId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase for buyer {BuyerId}", buyerId);
                return StatusCode(500, new { message = "An error occurred while creating the purchase" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PurchaseResponseDto>> UpdatePurchase(
            Guid id,
            [FromBody] PurchaseUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var purchase = await _purchaseService.UpdatePurchaseAsync(id, updateDto);
                if (purchase == null)
                {
                    return NotFound(new { message = "Purchase not found" });
                }

                return Ok(purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase {PurchaseId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the purchase" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePurchase(Guid id)
        {
            try
            {
                var result = await _purchaseService.DeletePurchaseAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Purchase not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting purchase {PurchaseId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the purchase" });
            }
        }
    }
}