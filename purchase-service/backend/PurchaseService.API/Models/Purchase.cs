using System.ComponentModel.DataAnnotations;

namespace PurchaseService.API.Models
{
    public class Purchase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BuyerId { get; set; }

        [Required]
        public Guid OfferId { get; set; }

        [Required]
        public Guid SellerId { get; set; }

        [Required]
        public decimal PurchaseAmount { get; set; }

        public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;

        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Offer details snapshot (for historical data)
        [StringLength(17)]
        public string? VIN { get; set; }

        [Required]
        [StringLength(100)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        public string? Condition { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }

    public enum PurchaseStatus
    {
        Pending = 1,
        Confirmed = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5,
        Refunded = 6
    }
}