using Microsoft.AspNetCore.Mvc;
using TransportService.API.Services;

namespace TransportService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseServiceClient _purchaseService;
        private readonly ILogger<PurchasesController> _logger;

        public PurchasesController(IPurchaseServiceClient purchaseService, ILogger<PurchasesController> logger)
        {
            _purchaseService = purchaseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchases(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] int? status = null,
            [FromQuery] string? make = null,
            [FromQuery] string? model = null,
            [FromQuery] int? year = null,
            [FromQuery] string? location = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var searchParams = new Dictionary<string, object?>
                {
                    ["page"] = page,
                    ["pageSize"] = pageSize,
                    ["status"] = status,
                    ["make"] = make,
                    ["model"] = model,
                    ["year"] = year,
                    ["location"] = location,
                    ["fromDate"] = fromDate,
                    ["toDate"] = toDate
                };

                var result = await _purchaseService.GetPurchasesAsync(searchParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases with search parameters");
                return StatusCode(500, new { message = "An error occurred while retrieving purchases" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchase(Guid id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseAsync(id);
                
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
    }
}