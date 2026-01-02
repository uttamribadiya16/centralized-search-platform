using System.ComponentModel.DataAnnotations;

namespace OfferService.API.Models
{
    public class Offer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SellerId { get; set; }

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

        public decimal? OfferAmount { get; set; }

        public string? Condition { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public OfferStatus Status { get; set; } = OfferStatus.Available;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum OfferStatus
    {
        Draft = 1,
        Available = 2,
        Pending = 3,
        Sold = 4,
        Withdrawn = 5,
        Expired = 6
    }

    public enum VehicleCondition
    {
        New = 1,
        Excellent = 2,
        Good = 3,
        Fair = 4,
        Poor = 5,
        Salvage = 6
    }
}