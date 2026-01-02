using System.ComponentModel.DataAnnotations;

namespace TransportService.API.Models
{
    public class Transport
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid CarrierId { get; set; }
        
        [Required]
        public Guid PurchaseId { get; set; }
        
        [Required]
        public Guid OfferId { get; set; }
        
        [Required]
        public Guid BuyerId { get; set; }
        
        [Required]
        public Guid SellerId { get; set; }
        
        public TransportStatus Status { get; set; } = TransportStatus.Assigned;
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
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
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties for related data (not stored in DB)
        public string? OfferVin { get; set; }
        public string? OfferMake { get; set; }
        public string? OfferModel { get; set; }
        public int? OfferYear { get; set; }
        public decimal? PurchaseAmount { get; set; }
    }
    
    public enum TransportStatus
    {
        Assigned = 1,
        PickupScheduled = 2,
        InTransit = 3,
        Delivered = 4,
        Cancelled = 5
    }
}