namespace PurchaseService.API.Models.DTOs
{
    public class PurchaseCreateDto
    {
        public Guid OfferId { get; set; }
        public decimal PurchaseAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class PurchaseResponseDto
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public Guid OfferId { get; set; }
        public Guid SellerId { get; set; }
        public decimal PurchaseAmount { get; set; }
        public PurchaseStatus Status { get; set; }
        public DateTime PurchasedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Notes { get; set; }
        
        // Offer details
        public string? VIN { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string? Condition { get; set; }
        public string? Address { get; set; }
    }

    public class PurchaseUpdateDto
    {
        public PurchaseStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class PurchaseSearchDto
    {
        public Guid? BuyerId { get; set; }
        public Guid? SellerId { get; set; }
        public PurchaseStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
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
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Condition { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}