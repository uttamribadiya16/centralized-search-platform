using Microsoft.EntityFrameworkCore;
using PurchaseService.API.Data;
using PurchaseService.API.Models;
using PurchaseService.API.Models.DTOs;

namespace PurchaseService.API.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly PurchaseDbContext _context;
        private readonly IOfferService _offerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(
            PurchaseDbContext context,
            IOfferService offerService,
            IEventPublisher eventPublisher,
            ILogger<PurchaseService> logger)
        {
            _context = context;
            _offerService = offerService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<PagedResult<PurchaseResponseDto>> GetPurchasesAsync(PurchaseSearchDto searchDto)
        {
            var query = _context.Purchases.AsQueryable();

            if (searchDto.BuyerId.HasValue)
                query = query.Where(p => p.BuyerId == searchDto.BuyerId.Value);

            if (searchDto.SellerId.HasValue)
                query = query.Where(p => p.SellerId == searchDto.SellerId.Value);

            if (searchDto.Status.HasValue)
                query = query.Where(p => p.Status == searchDto.Status.Value);

            if (searchDto.FromDate.HasValue)
                query = query.Where(p => p.PurchasedAt >= searchDto.FromDate.Value);

            if (searchDto.ToDate.HasValue)
                query = query.Where(p => p.PurchasedAt <= searchDto.ToDate.Value);

            if (!string.IsNullOrEmpty(searchDto.Make))
                query = query.Where(p => p.Make.Contains(searchDto.Make));

            if (!string.IsNullOrEmpty(searchDto.Model))
                query = query.Where(p => p.Model.Contains(searchDto.Model));

            if (searchDto.Year.HasValue)
                query = query.Where(p => p.Year == searchDto.Year.Value);

            var totalCount = await query.CountAsync();
            var purchases = await query
                .OrderByDescending(p => p.PurchasedAt)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return new PagedResult<PurchaseResponseDto>
            {
                Items = purchases.Select(MapToPurchaseResponseDto).ToList(),
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<PagedResult<PurchaseResponseDto>> GetPurchasesByBuyerAsync(Guid buyerId, PurchaseSearchDto searchDto)
        {
            searchDto.BuyerId = buyerId;
            return await GetPurchasesAsync(searchDto);
        }

        public async Task<PurchaseResponseDto?> GetPurchaseByIdAsync(Guid id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            return purchase == null ? null : MapToPurchaseResponseDto(purchase);
        }

        public async Task<PurchaseResponseDto> CreatePurchaseAsync(Guid buyerId, PurchaseCreateDto createDto)
        {
            // Get offer details from offer service
            var offer = await _offerService.GetOfferByIdAsync(createDto.OfferId);
            if (offer == null)
                throw new ArgumentException("Offer not found");

            if (offer.Status != "Available")
                throw new ArgumentException("Offer is not available for purchase");

            var purchase = new Purchase
            {
                BuyerId = buyerId,
                OfferId = createDto.OfferId,
                SellerId = offer.SellerId,
                PurchaseAmount = createDto.PurchaseAmount,
                Notes = createDto.Notes,
                // Copy offer details for historical data
                VIN = offer.VIN,
                Make = offer.Make,
                Model = offer.Model,
                Year = offer.Year,
                Condition = offer.Condition,
                Address = offer.Address,
                Status = PurchaseStatus.Pending
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Publish purchase event
            await _eventPublisher.PublishAsync("purchase.created", new
            {
                PurchaseId = purchase.Id,
                BuyerId = purchase.BuyerId,
                OfferId = purchase.OfferId,
                SellerId = purchase.SellerId,
                PurchaseAmount = purchase.PurchaseAmount,
                Make = purchase.Make,
                Model = purchase.Model,
                Year = purchase.Year,
                Status = purchase.Status.ToString(),
                PurchasedAt = purchase.PurchasedAt
            });

            _logger.LogInformation($"Purchase created: {purchase.Id}");

            return MapToPurchaseResponseDto(purchase);
        }

        public async Task<PurchaseResponseDto?> UpdatePurchaseAsync(Guid id, PurchaseUpdateDto updateDto)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return null;

            purchase.Status = updateDto.Status;
            purchase.Notes = updateDto.Notes;
            purchase.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Publish purchase updated event
            await _eventPublisher.PublishAsync("purchase.updated", new
            {
                PurchaseId = purchase.Id,
                BuyerId = purchase.BuyerId,
                OfferId = purchase.OfferId,
                SellerId = purchase.SellerId,
                Status = purchase.Status.ToString(),
                UpdatedAt = purchase.UpdatedAt
            });

            _logger.LogInformation($"Purchase updated: {purchase.Id}");

            return MapToPurchaseResponseDto(purchase);
        }

        public async Task<bool> DeletePurchaseAsync(Guid id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return false;

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            // Publish purchase deleted event
            await _eventPublisher.PublishAsync("purchase.deleted", new
            {
                PurchaseId = id,
                BuyerId = purchase.BuyerId,
                OfferId = purchase.OfferId,
                DeletedAt = DateTime.UtcNow
            });

            _logger.LogInformation($"Purchase deleted: {id}");
            return true;
        }

        private static PurchaseResponseDto MapToPurchaseResponseDto(Purchase purchase)
        {
            return new PurchaseResponseDto
            {
                Id = purchase.Id,
                BuyerId = purchase.BuyerId,
                OfferId = purchase.OfferId,
                SellerId = purchase.SellerId,
                PurchaseAmount = purchase.PurchaseAmount,
                Status = purchase.Status,
                PurchasedAt = purchase.PurchasedAt,
                CreatedAt = purchase.CreatedAt,
                UpdatedAt = purchase.UpdatedAt,
                Notes = purchase.Notes,
                VIN = purchase.VIN,
                Make = purchase.Make,
                Model = purchase.Model,
                Year = purchase.Year,
                Condition = purchase.Condition,
                Address = purchase.Address
            };
        }
    }
}