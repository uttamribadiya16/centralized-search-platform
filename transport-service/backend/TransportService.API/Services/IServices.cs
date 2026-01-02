using TransportService.API.Models.DTOs;

namespace TransportService.API.Services
{
    public interface IAuthService
    {
        Task<AuthenticatedUserDto?> AuthenticateUserAsync(string email, string password);
        Task<bool> ValidateCarrierAsync(Guid carrierId);
    }

    public interface ITransportService
    {
        Task<PagedResult<TransportResponseDto>> GetTransportsAsync(TransportSearchDto searchDto);
        Task<PagedResult<TransportResponseDto>> GetTransportsByCarrierAsync(Guid carrierId, TransportSearchDto searchDto);
        Task<TransportResponseDto?> GetTransportAsync(Guid id);
        Task<TransportResponseDto> CreateTransportAsync(Guid carrierId, TransportCreateDto createDto);
        Task<TransportResponseDto> AssignPurchaseToTransportAsync(Guid carrierId, TransportAssignmentDto assignmentDto);
        Task<TransportResponseDto?> UpdateTransportAsync(Guid id, TransportUpdateDto updateDto);
        Task<bool> DeleteTransportAsync(Guid id);
    }

    public interface IOfferServiceClient
    {
        Task<PagedResult<OfferDto>> GetOffersAsync(Dictionary<string, object?> searchParams);
        Task<OfferDto?> GetOfferAsync(Guid offerId);
    }

    public interface IPurchaseServiceClient
    {
        Task<PagedResult<PurchaseDto>> GetPurchasesAsync(Dictionary<string, object?> searchParams);
        Task<PurchaseDto?> GetPurchaseAsync(Guid purchaseId);
    }

    public interface IRabbitMQService
    {
        Task PublishTransportEventAsync(string eventType, object eventData);
    }
}