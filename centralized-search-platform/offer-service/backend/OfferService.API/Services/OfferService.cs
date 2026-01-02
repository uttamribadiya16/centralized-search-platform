using OfferService.API.Data;
using OfferService.API.Models;
using OfferService.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace OfferService.API.Services
{
    public interface IOfferService
    {
        Task<PagedResult<OfferResponseDto>> GetOffersAsync(OfferSearchDto searchDto);
        Task<PagedResult<OfferResponseDto>> GetOffersBySellerAsync(Guid sellerId, OfferSearchDto searchDto);
        Task<OfferResponseDto?> GetOfferByIdAsync(Guid id);
        Task<OfferResponseDto> CreateOfferAsync(CreateOfferDto createOfferDto);
        Task<OfferResponseDto?> UpdateOfferAsync(Guid id, UpdateOfferDto updateOfferDto);
        Task<bool> DeleteOfferAsync(Guid id);
        Task<bool> OfferExistsAsync(Guid id);
        Task<bool> VINExistsAsync(string vin, Guid? excludeOfferId = null);
        Task<List<OfferResponseDto>> GetFeaturedOffersAsync();
        Task<Dictionary<string, object>> GetSellerStatsAsync(Guid sellerId);
    }

    public class OfferService : IOfferService
    {
        private readonly OfferDbContext _context;
        private readonly ILogger<OfferService> _logger;
        private readonly IEventPublisher _eventPublisher;

        public OfferService(OfferDbContext context, ILogger<OfferService> logger, IEventPublisher eventPublisher)
        {
            _context = context;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<PagedResult<OfferResponseDto>> GetOffersAsync(OfferSearchDto searchDto)
        {
            var query = _context.Offers.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(o =>
                    o.Make.ToLower().Contains(searchTerm) ||
                    o.Model.ToLower().Contains(searchTerm) ||
                    o.VIN.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(searchDto.Make))
            {
                query = query.Where(o => o.Make.ToLower().Contains(searchDto.Make.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchDto.Model))
            {
                query = query.Where(o => o.Model.ToLower().Contains(searchDto.Model.ToLower()));
            }

            if (searchDto.MinYear.HasValue)
            {
                query = query.Where(o => o.Year >= searchDto.MinYear.Value);
            }

            if (searchDto.MaxYear.HasValue)
            {
                query = query.Where(o => o.Year <= searchDto.MaxYear.Value);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(o => o.OfferAmount >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(o => o.OfferAmount <= searchDto.MaxPrice.Value);
            }

            if (searchDto.Condition.HasValue)
            {
                query = query.Where(o => o.Condition == searchDto.Condition.Value.ToString());
            }

            if (searchDto.Status.HasValue)
            {
                query = query.Where(o => o.Status == searchDto.Status.Value);
            }

            // Apply sorting
            query = searchDto.SortBy.ToLower() switch
            {
                "price" => searchDto.SortDirection.ToLower() == "asc" 
                    ? query.OrderBy(o => o.OfferAmount) 
                    : query.OrderByDescending(o => o.OfferAmount),
                "year" => searchDto.SortDirection.ToLower() == "asc" 
                    ? query.OrderBy(o => o.Year) 
                    : query.OrderByDescending(o => o.Year),
                "make" => searchDto.SortDirection.ToLower() == "asc" 
                    ? query.OrderBy(o => o.Make) 
                    : query.OrderByDescending(o => o.Make),
                _ => searchDto.SortDirection.ToLower() == "asc" 
                    ? query.OrderBy(o => o.CreatedAt) 
                    : query.OrderByDescending(o => o.CreatedAt)
            };

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var offers = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

            return new PagedResult<OfferResponseDto>
            {
                Items = offers.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNext = searchDto.Page < totalPages,
                HasPrevious = searchDto.Page > 1
            };
        }

        public async Task<PagedResult<OfferResponseDto>> GetOffersBySellerAsync(Guid sellerId, OfferSearchDto searchDto)
        {
            var query = _context.Offers
                .Where(o => o.SellerId == sellerId)
                .AsQueryable();

            // Apply the same filtering as GetOffersAsync but for specific seller
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(o =>
                    o.Make.ToLower().Contains(searchTerm) ||
                    o.Model.ToLower().Contains(searchTerm) ||
                    o.VIN.ToLower().Contains(searchTerm));
            }

            if (searchDto.Status.HasValue)
            {
                query = query.Where(o => o.Status == searchDto.Status.Value);
            }

            // Apply sorting
            query = searchDto.SortBy.ToLower() switch
            {
                "price" => searchDto.SortDirection.ToLower() == "asc" 
                    ? query.OrderBy(o => o.OfferAmount) 
                    : query.OrderByDescending(o => o.OfferAmount),
                "year" => searchDto.SortDirection.ToLower() == "asc" 
                    ? query.OrderBy(o => o.Year) 
                    : query.OrderByDescending(o => o.Year),
                _ => searchDto.SortDirection.ToLower() == "asc" 
                    ? query.OrderBy(o => o.CreatedAt) 
                    : query.OrderByDescending(o => o.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var offers = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

            return new PagedResult<OfferResponseDto>
            {
                Items = offers.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNext = searchDto.Page < totalPages,
                HasPrevious = searchDto.Page > 1
            };
        }

        public async Task<OfferResponseDto?> GetOfferByIdAsync(Guid id)
        {
            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == id);

            return offer == null ? null : MapToResponseDto(offer);
        }

        public async Task<OfferResponseDto> CreateOfferAsync(CreateOfferDto createOfferDto)
        {
            // Check if VIN already exists
            var existingOffer = await _context.Offers
                .FirstOrDefaultAsync(o => o.VIN == createOfferDto.VIN);
            
            if (existingOffer != null)
            {
                throw new InvalidOperationException("A vehicle with this VIN already exists.");
            }

            var offer = new Offer
            {
                SellerId = createOfferDto.SellerId,
                VIN = createOfferDto.VIN,
                Make = createOfferDto.Make,
                Model = createOfferDto.Model,
                Year = createOfferDto.Year,
                OfferAmount = createOfferDto.OfferAmount,
                Condition = createOfferDto.Condition?.ToString(),
                Address = createOfferDto.Address
            };

            _context.Offers.Add(offer);
            
            await _context.SaveChangesAsync();

            // Return the created offer with generated ID
            var createdOffer = await _context.Offers
                .FirstAsync(o => o.Id == offer.Id);

            // Publish offer created event
            var eventData = new
            {
                Id = createdOffer.Id,
                SellerId = createdOffer.SellerId,
                VIN = createdOffer.VIN,
                Make = createdOffer.Make,
                Model = createdOffer.Model,
                Year = createdOffer.Year,
                OfferAmount = createdOffer.OfferAmount,
                Status = createdOffer.Status.ToString(),
                Condition = createdOffer.Condition ?? string.Empty,
                Address = createdOffer.Address ?? string.Empty,
                CreatedAt = createdOffer.CreatedAt,
                UpdatedAt = createdOffer.UpdatedAt,
                EventType = "Created"
            };
            
            await _eventPublisher.PublishOfferCreatedAsync(eventData);

            return MapToResponseDto(createdOffer);
        }

        public async Task<OfferResponseDto?> UpdateOfferAsync(Guid id, UpdateOfferDto updateOfferDto)
        {
            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == id);
            
            if (offer == null) return null;

            // Update only provided fields
            if (updateOfferDto.OfferAmount.HasValue)
                offer.OfferAmount = updateOfferDto.OfferAmount.Value;
            
            if (updateOfferDto.Condition.HasValue)
                offer.Condition = updateOfferDto.Condition.Value.ToString();
            
            if (!string.IsNullOrEmpty(updateOfferDto.Address))
                offer.Address = updateOfferDto.Address;
            
            if (updateOfferDto.Status.HasValue)
                offer.Status = updateOfferDto.Status.Value;

            await _context.SaveChangesAsync();

            // Reload with updated data
            var updatedOffer = await _context.Offers
                .FirstAsync(o => o.Id == offer.Id);

            // Publish offer updated event
            var eventData = new
            {
                Id = updatedOffer.Id,
                SellerId = updatedOffer.SellerId,
                VIN = updatedOffer.VIN,
                Make = updatedOffer.Make,
                Model = updatedOffer.Model,
                Year = updatedOffer.Year,
                OfferAmount = updatedOffer.OfferAmount,
                Status = updatedOffer.Status.ToString(),
                Condition = updatedOffer.Condition ?? string.Empty,
                Address = updatedOffer.Address ?? string.Empty,
                CreatedAt = updatedOffer.CreatedAt,
                UpdatedAt = updatedOffer.UpdatedAt,
                EventType = "Updated"
            };
            
            await _eventPublisher.PublishOfferUpdatedAsync(eventData);

            return MapToResponseDto(updatedOffer);
        }

        public async Task<bool> DeleteOfferAsync(Guid id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null) return false;

            var sellerId = offer.SellerId; // Store before deletion
            
            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            // Publish offer deleted event
            var eventData = new
            {
                Id = id,
                SellerId = sellerId,
                EventType = "Deleted"
            };
            
            await _eventPublisher.PublishOfferDeletedAsync(eventData);

            return true;
        }

        public async Task<bool> OfferExistsAsync(Guid id)
        {
            return await _context.Offers.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> VINExistsAsync(string vin, Guid? excludeOfferId = null)
        {
            var query = _context.Offers.Where(o => o.VIN == vin);
            
            if (excludeOfferId.HasValue)
            {
                query = query.Where(o => o.Id != excludeOfferId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<List<OfferResponseDto>> GetFeaturedOffersAsync()
        {
            var offers = await _context.Offers

                .Where(o => o.Status == OfferStatus.Available)
                .OrderByDescending(o => o.CreatedAt)
                .Take(6)
                .ToListAsync();

            return offers.Select(MapToResponseDto).ToList();
        }

        public async Task<Dictionary<string, object>> GetSellerStatsAsync(Guid sellerId)
        {
            var totalOffers = await _context.Offers.CountAsync(o => o.SellerId == sellerId);
            var activeOffers = await _context.Offers.CountAsync(o => o.SellerId == sellerId && o.Status == OfferStatus.Available);
            var soldOffers = await _context.Offers.CountAsync(o => o.SellerId == sellerId && o.Status == OfferStatus.Sold);
            var totalRevenue = await _context.Offers
                .Where(o => o.SellerId == sellerId && o.Status == OfferStatus.Sold)
                .SumAsync(o => (decimal?)o.OfferAmount) ?? 0;

            return new Dictionary<string, object>
            {
                { "TotalOffers", totalOffers },
                { "ActiveOffers", activeOffers },
                { "SoldOffers", soldOffers },
                { "TotalRevenue", totalRevenue }
            };
        }

        private OfferResponseDto MapToResponseDto(Offer offer)
        {
            return new OfferResponseDto
            {
                Id = offer.Id,
                SellerId = offer.SellerId,
                VIN = offer.VIN,
                Make = offer.Make,
                Model = offer.Model,
                Year = offer.Year,
                OfferAmount = offer.OfferAmount,
                Condition = offer.Condition,
                Address = offer.Address,
                Status = offer.Status.ToString(),
                CreatedAt = offer.CreatedAt,
                UpdatedAt = offer.UpdatedAt
            };
        }
    }
}