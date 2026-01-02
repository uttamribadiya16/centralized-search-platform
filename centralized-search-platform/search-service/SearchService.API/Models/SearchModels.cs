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

public class SearchRequest
{
    public Guid SellerId { get; set; }
    public string SearchText { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SearchResponse
{
    public List<OfferDocument> Results { get; set; } = new();
    public long TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}