using Nest;

namespace SearchService.API.Models;

[ElasticsearchType(RelationName = "offers")]
public class OfferDocument
{
    [Keyword]
    public Guid Id { get; set; }
    
    [Keyword]
    public Guid SellerId { get; set; }
    
    [Keyword]
    public string VIN { get; set; } = string.Empty;
    
    [Text(Analyzer = "standard")]
    public string Make { get; set; } = string.Empty;
    
    [Text(Analyzer = "standard")]
    public string Model { get; set; } = string.Empty;
    
    [Number]
    public int Year { get; set; }
    
    [Number]
    public decimal OfferAmount { get; set; }
    
    [Keyword]
    public string Status { get; set; } = string.Empty;
    
    [Text(Analyzer = "standard")]
    public string Condition { get; set; } = string.Empty;
    
    [Text(Analyzer = "standard")]
    public string Address { get; set; } = string.Empty;
    
    [Date]
    public DateTime CreatedAt { get; set; }
    
    [Date]
    public DateTime UpdatedAt { get; set; }
    
    // Combined searchable text for full-text search
    [Text(Analyzer = "standard")]
    public string SearchText => $"{Make} {Model} {Year} {VIN} {Condition} {Address}".ToLowerInvariant();
}

public class OfferEvent
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal OfferAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string EventType { get; set; } = string.Empty; // "Created" or "Updated"
}

[ElasticsearchType(RelationName = "purchases")]
public class PurchaseDocument
{
    [Keyword]
    public Guid Id { get; set; }
    
    [Keyword]
    public Guid BuyerId { get; set; }
    
    [Keyword]
    public Guid OfferId { get; set; }
    
    [Keyword]
    public Guid SellerId { get; set; }
    
    [Number]
    public decimal PurchaseAmount { get; set; }
    
    [Keyword]
    public string Status { get; set; } = string.Empty;
    
    [Date]
    public DateTime PurchasedAt { get; set; }
    
    [Text(Analyzer = "standard")]
    public string Make { get; set; } = string.Empty;
    
    [Text(Analyzer = "standard")]
    public string Model { get; set; } = string.Empty;
    
    [Number]
    public int Year { get; set; }
    
    // Combined searchable text for full-text search
    [Text(Analyzer = "standard")]
    public string SearchText => $"{Make} {Model} {Year} {Status}".ToLowerInvariant();
}

public class PurchaseEvent
{
    public Guid PurchaseId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid OfferId { get; set; }
    public Guid SellerId { get; set; }
    public decimal PurchaseAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PurchasedAt { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}

[ElasticsearchType(RelationName = "transports")]
public class TransportDocument
{
    [Keyword]
    public Guid Id { get; set; }
    
    [Keyword]
    public Guid CarrierId { get; set; }
    
    [Keyword]
    public Guid PurchaseId { get; set; }
    
    [Keyword]
    public Guid OfferId { get; set; }
    
    [Keyword]
    public Guid BuyerId { get; set; }
    
    [Keyword]
    public Guid SellerId { get; set; }
    
    [Keyword]
    public string Status { get; set; } = string.Empty;
    
    [Date]
    public DateTime AssignedAt { get; set; }
    
    [Date]
    public DateTime? UpdatedAt { get; set; }
    
    [Number(NumberType.Double)]
    public decimal? TransportFee { get; set; }
    
    [Text(Analyzer = "standard")]
    public string? PickupAddress { get; set; }
    
    [Text(Analyzer = "standard")]
    public string? DeliveryAddress { get; set; }
    
    // Vehicle information for search
    [Keyword]
    public string? Vin { get; set; }
    
    [Text(Analyzer = "standard")]
    public string? Make { get; set; }
    
    [Text(Analyzer = "standard")]
    public string? Model { get; set; }
    
    [Number(NumberType.Integer)]
    public int? Year { get; set; }
    
    // Combined searchable text for full-text search
    [Text(Analyzer = "standard")]
    public string SearchText => $"{Status} {Make} {Model} {Year} {Vin} {PickupAddress} {DeliveryAddress}".ToLowerInvariant();
}

public class TransportEvent
{
    public Guid TransportId { get; set; }
    public Guid CarrierId { get; set; }
    public Guid PurchaseId { get; set; }
    public Guid OfferId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal? TransportFee { get; set; }
    public string? PickupAddress { get; set; }
    public string? DeliveryAddress { get; set; }
    
    // Vehicle details from the purchase/offer
    public VehicleDetails? VehicleDetails { get; set; }
}

public class VehicleDetails
{
    public string? Vin { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
}

public class SearchRequest
{
    public Guid SellerId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid CarrierId { get; set; }
    public string SearchText { get; set; } = string.Empty;
    public string SearchType { get; set; } = "offers"; // "offers", "purchases", "transports", "all"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SearchResponse
{
    public List<OfferDocument> OfferResults { get; set; } = new();
    public List<PurchaseDocument> PurchaseResults { get; set; } = new();
    public List<TransportDocument> TransportResults { get; set; } = new();
    public long TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}