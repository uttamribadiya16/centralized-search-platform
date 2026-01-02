using Microsoft.EntityFrameworkCore;
using TransportService.API.Data;
using TransportService.API.Models;
using TransportService.API.Models.DTOs;

namespace TransportService.API.Services
{
    public class TransportService : ITransportService
    {
        private readonly TransportDbContext _context;
        private readonly IOfferServiceClient _offerService;
        private readonly IPurchaseServiceClient _purchaseService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILogger<TransportService> _logger;

        public TransportService(
            TransportDbContext context,
            IOfferServiceClient offerService,
            IPurchaseServiceClient purchaseService,
            IRabbitMQService rabbitMQService,
            ILogger<TransportService> logger)
        {
            _context = context;
            _offerService = offerService;
            _purchaseService = purchaseService;
            _rabbitMQService = rabbitMQService;
            _logger = logger;
        }

        public async Task<PagedResult<TransportResponseDto>> GetTransportsAsync(TransportSearchDto searchDto)
        {
            var query = _context.Transports.AsQueryable();

            if (searchDto.Status.HasValue)
            {
                query = query.Where(t => t.Status == searchDto.Status.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(t => t.AssignedAt >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                query = query.Where(t => t.AssignedAt <= searchDto.ToDate.Value);
            }

            var totalCount = await query.CountAsync();
            var transports = await query
                .OrderByDescending(t => t.AssignedAt)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var transportDtos = await EnrichTransportsWithExternalData(transports);

            return new PagedResult<TransportResponseDto>
            {
                Items = transportDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };
        }

        public async Task<PagedResult<TransportResponseDto>> GetTransportsByCarrierAsync(Guid carrierId, TransportSearchDto searchDto)
        {
            var query = _context.Transports.Where(t => t.CarrierId == carrierId);

            if (searchDto.Status.HasValue)
            {
                query = query.Where(t => t.Status == searchDto.Status.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(t => t.AssignedAt >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                query = query.Where(t => t.AssignedAt <= searchDto.ToDate.Value);
            }

            var totalCount = await query.CountAsync();
            var transports = await query
                .OrderByDescending(t => t.AssignedAt)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var transportDtos = await EnrichTransportsWithExternalData(transports);

            return new PagedResult<TransportResponseDto>
            {
                Items = transportDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };
        }

        public async Task<TransportResponseDto?> GetTransportAsync(Guid id)
        {
            var transport = await _context.Transports.FindAsync(id);
            if (transport == null)
            {
                return null;
            }

            var enrichedTransports = await EnrichTransportsWithExternalData(new List<Transport> { transport });
            return enrichedTransports.FirstOrDefault();
        }

        public async Task<TransportResponseDto> CreateTransportAsync(Guid carrierId, TransportCreateDto createDto)
        {
            // Get purchase details to extract offer and buyer info
            var purchase = await _purchaseService.GetPurchaseAsync(createDto.PurchaseId);
            if (purchase == null)
            {
                throw new ArgumentException("Purchase not found");
            }

            var transport = new Transport
            {
                CarrierId = carrierId,
                PurchaseId = createDto.PurchaseId,
                OfferId = purchase.OfferId,
                BuyerId = purchase.BuyerId,
                SellerId = purchase.SellerId,
                TransportFee = createDto.TransportFee,
                PickupScheduledAt = createDto.PickupScheduledAt,
                Notes = createDto.Notes,
                PickupAddress = createDto.PickupAddress,
                DeliveryAddress = createDto.DeliveryAddress
            };

            _context.Transports.Add(transport);
            await _context.SaveChangesAsync();

            // Publish transport created event
            await _rabbitMQService.PublishTransportEventAsync("transport.created", new
            {
                TransportId = transport.Id,
                CarrierId = transport.CarrierId,
                PurchaseId = transport.PurchaseId,
                OfferId = transport.OfferId,
                Status = transport.Status.ToString(),
                AssignedAt = transport.AssignedAt
            });

            var enrichedTransports = await EnrichTransportsWithExternalData(new List<Transport> { transport });
            return enrichedTransports.First();
        }

        public async Task<TransportResponseDto?> UpdateTransportAsync(Guid id, TransportUpdateDto updateDto)
        {
            var transport = await _context.Transports.FindAsync(id);
            if (transport == null)
            {
                return null;
            }

            if (updateDto.Status.HasValue)
                transport.Status = updateDto.Status.Value;
            
            if (updateDto.PickupScheduledAt.HasValue)
                transport.PickupScheduledAt = updateDto.PickupScheduledAt.Value;
            
            if (updateDto.PickedUpAt.HasValue)
                transport.PickedUpAt = updateDto.PickedUpAt.Value;
            
            if (updateDto.DeliveredAt.HasValue)
                transport.DeliveredAt = updateDto.DeliveredAt.Value;
            
            if (updateDto.TransportFee.HasValue)
                transport.TransportFee = updateDto.TransportFee.Value;
            
            if (!string.IsNullOrEmpty(updateDto.Notes))
                transport.Notes = updateDto.Notes;
            
            if (!string.IsNullOrEmpty(updateDto.PickupAddress))
                transport.PickupAddress = updateDto.PickupAddress;
            
            if (!string.IsNullOrEmpty(updateDto.DeliveryAddress))
                transport.DeliveryAddress = updateDto.DeliveryAddress;

            transport.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Publish transport updated event
            await _rabbitMQService.PublishTransportEventAsync("transport.updated", new
            {
                TransportId = transport.Id,
                CarrierId = transport.CarrierId,
                PurchaseId = transport.PurchaseId,
                OfferId = transport.OfferId,
                Status = transport.Status.ToString(),
                UpdatedAt = transport.UpdatedAt
            });

            var enrichedTransports = await EnrichTransportsWithExternalData(new List<Transport> { transport });
            return enrichedTransports.First();
        }

        public async Task<bool> DeleteTransportAsync(Guid id)
        {
            var transport = await _context.Transports.FindAsync(id);
            if (transport == null)
            {
                return false;
            }

            _context.Transports.Remove(transport);
            await _context.SaveChangesAsync();

            // Publish transport deleted event
            await _rabbitMQService.PublishTransportEventAsync("transport.deleted", new
            {
                TransportId = transport.Id,
                CarrierId = transport.CarrierId,
                PurchaseId = transport.PurchaseId,
                OfferId = transport.OfferId
            });

            return true;
        }

        public async Task<TransportResponseDto> AssignPurchaseToTransportAsync(Guid carrierId, TransportAssignmentDto assignmentDto)
        {
            // Verify purchase exists
            var purchase = await _purchaseService.GetPurchaseAsync(assignmentDto.PurchaseId);
            if (purchase == null)
            {
                throw new ArgumentException("Purchase not found");
            }

            // Check if transport already exists for this purchase
            var existingTransport = await _context.Transports
                .FirstOrDefaultAsync(t => t.PurchaseId == assignmentDto.PurchaseId);
            
            if (existingTransport != null)
            {
                throw new InvalidOperationException("Transport already exists for this purchase");
            }

            var transport = new Transport
            {
                CarrierId = carrierId,
                PurchaseId = assignmentDto.PurchaseId,
                OfferId = purchase.OfferId,
                BuyerId = purchase.BuyerId,
                SellerId = purchase.SellerId,
                TransportFee = assignmentDto.TransportFee ?? 0,
                PickupScheduledAt = assignmentDto.EstimatedDeliveryDate,
                Notes = assignmentDto.Notes,
                PickupAddress = assignmentDto.OriginLocation,
                DeliveryAddress = assignmentDto.DestinationLocation
            };

            _context.Transports.Add(transport);
            await _context.SaveChangesAsync();

            // Publish transport assignment event for search indexing
            await _rabbitMQService.PublishTransportEventAsync("transport.assigned", new
            {
                TransportId = transport.Id,
                CarrierId = transport.CarrierId,
                PurchaseId = transport.PurchaseId,
                OfferId = transport.OfferId,
                BuyerId = transport.BuyerId,
                SellerId = purchase.SellerId,
                TransportFee = transport.TransportFee,
                Status = transport.Status.ToString(),
                AssignedAt = transport.AssignedAt,
                PickupAddress = transport.PickupAddress,
                DeliveryAddress = transport.DeliveryAddress,
                VehicleDetails = new
                {
                    Vin = purchase.Vin,
                    Make = purchase.Make,
                    Model = purchase.Model,
                    Year = purchase.Year
                }
            });

            var enrichedTransports = await EnrichTransportsWithExternalData(new List<Transport> { transport });
            return enrichedTransports.First();
        }

        private async Task<List<TransportResponseDto>> EnrichTransportsWithExternalData(List<Transport> transports)
        {
            var result = new List<TransportResponseDto>();

            foreach (var transport in transports)
            {
                var dto = new TransportResponseDto
                {
                    Id = transport.Id,
                    CarrierId = transport.CarrierId,
                    PurchaseId = transport.PurchaseId,
                    OfferId = transport.OfferId,
                    BuyerId = transport.BuyerId,
                    SellerId = transport.SellerId,
                    Status = transport.Status,
                    AssignedAt = transport.AssignedAt,
                    PickupScheduledAt = transport.PickupScheduledAt,
                    PickedUpAt = transport.PickedUpAt,
                    DeliveredAt = transport.DeliveredAt,
                    TransportFee = transport.TransportFee,
                    Notes = transport.Notes,
                    PickupAddress = transport.PickupAddress,
                    DeliveryAddress = transport.DeliveryAddress,
                    CreatedAt = transport.CreatedAt,
                    UpdatedAt = transport.UpdatedAt
                };

                // Get purchase details
                try
                {
                    var purchase = await _purchaseService.GetPurchaseAsync(transport.PurchaseId);
                    if (purchase != null)
                    {
                        dto.PurchaseAmount = purchase.PurchaseAmount;
                        dto.OfferVin = purchase.Vin;
                        dto.OfferMake = purchase.Make;
                        dto.OfferModel = purchase.Model;
                        dto.OfferYear = purchase.Year;
                        dto.OfferAddress = purchase.Address;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get purchase details for transport {TransportId}", transport.Id);
                }

                result.Add(dto);
            }

            return result;
        }
    }
}