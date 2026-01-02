using PurchaseService.API.Models.DTOs;

namespace PurchaseService.API.Services
{
    public interface IOfferService
    {
        Task<OfferResponseDto?> GetOfferByIdAsync(Guid offerId);
        Task<PagedResult<OfferResponseDto>> GetAvailableOffersAsync(OfferSearchDto searchDto);
    }
}