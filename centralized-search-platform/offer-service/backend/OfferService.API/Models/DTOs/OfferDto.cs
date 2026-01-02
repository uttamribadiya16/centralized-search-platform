using System.ComponentModel.DataAnnotations;

namespace OfferService.API.Models.DTOs
{
    public class CreateOfferDto
    {
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
        [Range(1900, 2030)]
        public int Year { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? OfferAmount { get; set; }

        public VehicleCondition? Condition { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }

    public class UpdateOfferDto
    {
        [Range(0.01, double.MaxValue)]
        public decimal? OfferAmount { get; set; }

        public VehicleCondition? Condition { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public OfferStatus? Status { get; set; }
    }

    public class OfferResponseDto
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public string? VIN { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal? OfferAmount { get; set; }
        public string? Condition { get; set; }
        public string? Address { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OfferSearchDto
    {
        public string? SearchTerm { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public VehicleCondition? Condition { get; set; }
        public OfferStatus? Status { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}