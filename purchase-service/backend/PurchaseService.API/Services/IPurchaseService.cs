using PurchaseService.API.Models.DTOs;

namespace PurchaseService.API.Services
{
    public interface IPurchaseService
    {
        Task<PagedResult<PurchaseResponseDto>> GetPurchasesAsync(PurchaseSearchDto searchDto);
        Task<PagedResult<PurchaseResponseDto>> GetPurchasesByBuyerAsync(Guid buyerId, PurchaseSearchDto searchDto);
        Task<PurchaseResponseDto?> GetPurchaseByIdAsync(Guid id);
        Task<PurchaseResponseDto> CreatePurchaseAsync(Guid buyerId, PurchaseCreateDto createDto);
        Task<PurchaseResponseDto?> UpdatePurchaseAsync(Guid id, PurchaseUpdateDto updateDto);
        Task<bool> DeletePurchaseAsync(Guid id);
    }
}