using System.ComponentModel.DataAnnotations;

namespace TransportService.API.Models.DTOs
{
    // Auth DTOs
    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class AuthenticatedUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class AccountUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int UserType { get; set; }
        public int Status { get; set; }
    }

    // Transport DTOs
    public class TransportAssignmentDto
    {
        [Required]
        public Guid PurchaseId { get; set; }
        
        [Required]
        public Guid CarrierId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string OriginLocation { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string DestinationLocation { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
        
        public DateTime? EstimatedDeliveryDate { get; set; }
        
        public decimal? TransportFee { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class TransportCreateDto
    {
        [Required]
        public Guid PurchaseId { get; set; }
        
        public decimal? TransportFee { get; set; }
        
        public DateTime? PickupScheduledAt { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? PickupAddress { get; set; }
        
        [StringLength(500)]
        public string? DeliveryAddress { get; set; }
    }

    public class TransportUpdateDto
    {
        public TransportStatus? Status { get; set; }
        
        public DateTime? PickupScheduledAt { get; set; }
        
        public DateTime? PickedUpAt { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        public decimal? TransportFee { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(500)]
        public string? PickupAddress { get; set; }
        
        [StringLength(500)]
        public string? DeliveryAddress { get; set; }
    }

    public class TransportResponseDto
    {
        public Guid Id { get; set; }
        public Guid CarrierId { get; set; }
        public Guid PurchaseId { get; set; }
        public Guid OfferId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }
        public TransportStatus Status { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? PickupScheduledAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public decimal? TransportFee { get; set; }
        public string? Notes { get; set; }
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Related data
        public string? OfferVin { get; set; }
        public string? OfferMake { get; set; }
        public string? OfferModel { get; set; }
        public int? OfferYear { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public string? OfferAddress { get; set; }
    }

    public class TransportSearchDto
    {
        public TransportStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchText { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // External service DTOs
    public class OfferDto
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal OfferAmount { get; set; }
        public string? Condition { get; set; }
        public string? Address { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PurchaseDto
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public Guid OfferId { get; set; }
        public Guid SellerId { get; set; }
        public decimal PurchaseAmount { get; set; }
        public int Status { get; set; }
        public DateTime PurchasedAt { get; set; }
        public string? Notes { get; set; }
        public string? Vin { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Condition { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}