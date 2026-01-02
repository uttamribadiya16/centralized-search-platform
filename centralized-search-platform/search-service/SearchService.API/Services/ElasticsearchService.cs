using Nest;
using SearchService.API.Models;

namespace SearchService.API.Services;

public interface IElasticsearchService
{
    Task<bool> InitializeIndexAsync();
    Task<bool> IndexOfferAsync(OfferDocument offer);
    Task<SearchResponse> SearchOffersAsync(Models.SearchRequest request);
    Task<bool> DeleteOfferAsync(Guid offerId);
}

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticClient _client;
    private const string IndexName = "offers";
    private readonly ILogger<ElasticsearchService> _logger;

    public ElasticsearchService(string connectionString)
    {
        var settings = new ConnectionSettings(new Uri(connectionString))
            .DefaultIndex(IndexName)
            .DisableDirectStreaming();

        _client = new ElasticClient(settings);
        
        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<ElasticsearchService>();
    }

    public async Task<bool> InitializeIndexAsync()
    {
        try
        {
            var existsResponse = await _client.Indices.ExistsAsync(IndexName);
            if (existsResponse.Exists)
            {
                _logger.LogInformation("Index {IndexName} already exists", IndexName);
                return true;
            }

            var createResponse = await _client.Indices.CreateAsync(IndexName, c => c
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                )
                .Map<OfferDocument>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Keyword(k => k.Name(n => n.SellerId))
                        .Keyword(k => k.Name(n => n.VIN))
                        .Text(t => t.Name(n => n.Make).Analyzer("standard"))
                        .Text(t => t.Name(n => n.Model).Analyzer("standard"))
                        .Number(n => n.Name(nm => nm.Year))
                        .Number(n => n.Name(nm => nm.OfferAmount))
                        .Keyword(k => k.Name(n => n.Status))
                        .Text(t => t.Name(n => n.Condition).Analyzer("standard"))
                        .Text(t => t.Name(n => n.Address).Analyzer("standard"))
                        .Date(d => d.Name(n => n.CreatedAt))
                        .Date(d => d.Name(n => n.UpdatedAt))
                        .Text(t => t.Name(n => n.SearchText).Analyzer("standard"))
                    )
                )
            );

            if (createResponse.IsValid)
            {
                _logger.LogInformation("Successfully created index {IndexName}", IndexName);
                return true;
            }
            else
            {
                _logger.LogError("Failed to create index {IndexName}: {Error}", IndexName, createResponse.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Elasticsearch index");
            return false;
        }
    }

    public async Task<bool> IndexOfferAsync(OfferDocument offer)
    {
        try
        {
            var response = await _client.IndexDocumentAsync(offer);
            
            if (response.IsValid)
            {
                _logger.LogInformation("Successfully indexed offer {OfferId} for seller {SellerId}", offer.Id, offer.SellerId);
                return true;
            }
            else
            {
                _logger.LogError("Failed to index offer {OfferId}: {Error}", offer.Id, response.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing offer {OfferId}", offer.Id);
            return false;
        }
    }

    public async Task<SearchResponse> SearchOffersAsync(Models.SearchRequest request)
    {
        try
        {
            var searchRequest = new SearchRequest<OfferDocument>
            {
                From = (request.Page - 1) * request.PageSize,
                Size = request.PageSize,
                Query = BuildQuery(request),
                Sort = new List<ISort>
                {
                    new FieldSort { Field = Infer.Field<OfferDocument>(f => f.CreatedAt), Order = SortOrder.Descending }
                }
            };

            var response = await _client.SearchAsync<OfferDocument>(searchRequest);

            if (response.IsValid)
            {
                return new SearchResponse
                {
                    Results = response.Documents.ToList(),
                    TotalCount = response.Total,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
            else
            {
                _logger.LogError("Search failed: {Error}", response.DebugInformation);
                return new SearchResponse();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search for seller {SellerId}", request.SellerId);
            return new SearchResponse();
        }
    }

    public async Task<bool> DeleteOfferAsync(Guid offerId)
    {
        try
        {
            var response = await _client.DeleteAsync<OfferDocument>(offerId);
            
            if (response.IsValid)
            {
                _logger.LogInformation("Successfully deleted offer {OfferId} from index", offerId);
                return true;
            }
            else
            {
                _logger.LogError("Failed to delete offer {OfferId}: {Error}", offerId, response.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting offer {OfferId}", offerId);
            return false;
        }
    }

    private QueryContainer BuildQuery(Models.SearchRequest request)
    {
        var mustQueries = new List<QueryContainer>
        {
            // Always filter by seller ID - security constraint
            new TermQuery { Field = Infer.Field<OfferDocument>(f => f.SellerId), Value = request.SellerId }
        };

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.Trim();
            
            var searchQueries = new List<QueryContainer>
            {
                // Search in the combined search text field
                new MultiMatchQuery
                {
                    Fields = Infer.Fields<OfferDocument>(f => f.SearchText),
                    Query = searchText,
                    Operator = Operator.And,
                    Boost = 1.0
                },
                // Search in specific fields with different boosts
                new MultiMatchQuery
                {
                    Fields = Infer.Fields<OfferDocument>(f => f.Make, f => f.Model, f => f.VIN),
                    Query = searchText,
                    Operator = Operator.Or,
                    Boost = 2.0
                }
            };

            mustQueries.Add(new BoolQuery
            {
                Should = searchQueries,
                MinimumShouldMatch = 1
            });
        }

        return new BoolQuery
        {
            Must = mustQueries
        };
    }
}