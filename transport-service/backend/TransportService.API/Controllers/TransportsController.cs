using Microsoft.AspNetCore.Mvc;
using TransportService.API.Models.DTOs;
using TransportService.API.Services;

namespace TransportService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportsController : ControllerBase
    {
        private readonly Services.ITransportService _transportService;
        private readonly ILogger<TransportsController> _logger;

        public TransportsController(Services.ITransportService transportService, ILogger<TransportsController> logger)
        {
            _transportService = transportService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransports([FromQuery] TransportSearchDto searchDto)
        {
            try
            {
                var result = await _transportService.GetTransportsAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transports");
                return StatusCode(500, new { message = "An error occurred while retrieving transports" });
            }
        }

        [HttpGet("carrier/{carrierId}")]
        public async Task<IActionResult> GetTransportsByCarrier(Guid carrierId, [FromQuery] TransportSearchDto searchDto)
        {
            try
            {
                var result = await _transportService.GetTransportsByCarrierAsync(carrierId, searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transports for carrier {CarrierId}", carrierId);
                return StatusCode(500, new { message = "An error occurred while retrieving carrier transports" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransport(Guid id)
        {
            try
            {
                var transport = await _transportService.GetTransportAsync(id);
                
                if (transport == null)
                {
                    return NotFound(new { message = "Transport not found" });
                }

                return Ok(transport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transport {TransportId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the transport" });
            }
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignPurchaseToTransport([FromBody] TransportAssignmentDto assignmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input data" });
                }

                var transport = await _transportService.AssignPurchaseToTransportAsync(assignmentDto.CarrierId, assignmentDto);
                return CreatedAtAction(nameof(GetTransport), new { id = transport.Id }, transport);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data for transport assignment");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning purchase {PurchaseId} to transport for carrier {CarrierId}", assignmentDto.PurchaseId, assignmentDto.CarrierId);
                return StatusCode(500, new { message = "An error occurred while creating the transport assignment" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransport(Guid id, [FromBody] TransportUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input data" });
                }

                var transport = await _transportService.UpdateTransportAsync(id, updateDto);
                
                if (transport == null)
                {
                    return NotFound(new { message = "Transport not found" });
                }

                return Ok(transport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transport {TransportId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the transport" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransport(Guid id)
        {
            try
            {
                var deleted = await _transportService.DeleteTransportAsync(id);
                
                if (!deleted)
                {
                    return NotFound(new { message = "Transport not found" });
                }

                return Ok(new { message = "Transport deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transport {TransportId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the transport" });
            }
        }
    }
}