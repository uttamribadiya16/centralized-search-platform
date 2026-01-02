using Nest;
using SearchService.API.Models;

namespace SearchService.API.Services;

public interface IElasticsearchService
{
    Task<bool> InitializeIndexAsync();
    Task<bool> IndexOfferAsync(OfferDocument offer);
    Task<SearchResponse> SearchOffersAsync(Models.SearchRequest request);
    Task<bool> DeleteOfferAsync(Guid offerId);
    Task<bool> IndexPurchaseAsync(PurchaseDocument purchase);
    Task<SearchResponse> SearchPurchasesAsync(Models.SearchRequest request);
    Task<bool> DeletePurchaseAsync(Guid purchaseId);
    Task<bool> IndexTransportAsync(TransportDocument transport);
    Task<SearchResponse> SearchTransportsAsync(Models.SearchRequest request);
    Task<bool> DeleteTransportAsync(Guid transportId);
    Task<SearchResponse> SearchAllAsync(Models.SearchRequest request);
}

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticClient _client;
    private const string OffersIndexName = "offers";
    private const string PurchasesIndexName = "purchases";
    private const string TransportsIndexName = "transports";
    private readonly ILogger<ElasticsearchService> _logger;

    public ElasticsearchService(string connectionString)
    {
        var settings = new ConnectionSettings(new Uri(connectionString))
            .DefaultIndex(OffersIndexName)
            .DisableDirectStreaming();

        _client = new ElasticClient(settings);
        
        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<ElasticsearchService>();
    }

    public async Task<bool> InitializeIndexAsync()
    {
        try
        {
            // Initialize offers index
            var offersExists = await _client.Indices.ExistsAsync(OffersIndexName);
            if (!offersExists.Exists)
            {
                var offersCreateResponse = await _client.Indices.CreateAsync(OffersIndexName, c => c
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

                if (!offersCreateResponse.IsValid)
                {
                    _logger.LogError("Failed to create offers index: {Error}", offersCreateResponse.DebugInformation);
                    return false;
                }
                _logger.LogInformation("Successfully created offers index");
            }
            else
            {
                _logger.LogInformation("Offers index already exists");
            }

            // Initialize purchases index
            var purchasesExists = await _client.Indices.ExistsAsync(PurchasesIndexName);
            if (!purchasesExists.Exists)
            {
                var purchasesCreateResponse = await _client.Indices.CreateAsync(PurchasesIndexName, c => c
                    .Settings(s => s
                        .NumberOfShards(1)
                        .NumberOfReplicas(0)
                    )
                    .Map<PurchaseDocument>(m => m
                        .AutoMap()
                        .Properties(p => p
                            .Keyword(k => k.Name(n => n.Id))
                            .Keyword(k => k.Name(n => n.BuyerId))
                            .Keyword(k => k.Name(n => n.OfferId))
                            .Keyword(k => k.Name(n => n.SellerId))
                            .Number(n => n.Name(nm => nm.PurchaseAmount))
                            .Keyword(k => k.Name(n => n.Status))
                            .Date(d => d.Name(n => n.PurchasedAt))
                            .Text(t => t.Name(n => n.Make).Analyzer("standard"))
                            .Text(t => t.Name(n => n.Model).Analyzer("standard"))
                            .Number(n => n.Name(nm => nm.Year))
                            .Text(t => t.Name(n => n.SearchText).Analyzer("standard"))
                        )
                    )
                );

                if (!purchasesCreateResponse.IsValid)
                {
                    _logger.LogError("Failed to create purchases index: {Error}", purchasesCreateResponse.DebugInformation);
                    return false;
                }
                _logger.LogInformation("Successfully created purchases index");
            }
            else
            {
                _logger.LogInformation("Purchases index already exists");
            }

            // Initialize transports index
            var transportsExists = await _client.Indices.ExistsAsync(TransportsIndexName);
            if (!transportsExists.Exists)
            {
                var transportsCreateResponse = await _client.Indices.CreateAsync(TransportsIndexName, c => c
                    .Settings(s => s
                        .NumberOfShards(1)
                        .NumberOfReplicas(0)
                    )
                    .Map<TransportDocument>(m => m
                        .AutoMap()
                        .Properties(p => p
                            .Keyword(k => k.Name(n => n.Id))
                            .Keyword(k => k.Name(n => n.CarrierId))
                            .Keyword(k => k.Name(n => n.PurchaseId))
                            .Keyword(k => k.Name(n => n.OfferId))
                            .Keyword(k => k.Name(n => n.Status))
                            .Date(d => d.Name(n => n.AssignedAt))
                            .Date(d => d.Name(n => n.UpdatedAt))
                            .Text(t => t.Name(n => n.SearchText).Analyzer("standard"))
                        )
                    )
                );

                if (!transportsCreateResponse.IsValid)
                {
                    _logger.LogError("Failed to create transports index: {Error}", transportsCreateResponse.DebugInformation);
                    return false;
                }
                _logger.LogInformation("Successfully created transports index");
            }
            else
            {
                _logger.LogInformation("Transports index already exists");
            }

            return true;
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
                    OfferResults = response.Documents.ToList(),
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

    public async Task<bool> IndexPurchaseAsync(PurchaseDocument purchase)
    {
        try
        {
            var response = await _client.IndexAsync(purchase, idx => idx
                .Index(PurchasesIndexName)
                .Id(purchase.Id)
                .Refresh(Elasticsearch.Net.Refresh.WaitFor)
            );

            if (response.IsValid)
            {
                _logger.LogInformation("Successfully indexed purchase {PurchaseId}", purchase.Id);
                return true;
            }
            else
            {
                _logger.LogError("Failed to index purchase {PurchaseId}: {Error}", 
                    purchase.Id, response.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing purchase {PurchaseId}", purchase.Id);
            return false;
        }
    }

    public async Task<SearchResponse> SearchPurchasesAsync(Models.SearchRequest request)
    {
        try
        {
            var query = BuildPurchaseQuery(request);
            
            var searchResponse = await _client.SearchAsync<PurchaseDocument>(s => s
                .Index(PurchasesIndexName)
                .Query(q => query)
                .From((request.Page - 1) * request.PageSize)
                .Size(request.PageSize)
                .Sort(sort => sort.Descending(p => p.PurchasedAt))
            );

            if (searchResponse.IsValid)
            {
                return new SearchResponse
                {
                    PurchaseResults = searchResponse.Documents.ToList(),
                    TotalCount = searchResponse.Total,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
            else
            {
                _logger.LogError("Purchase search failed: {Error}", searchResponse.DebugInformation);
                return new SearchResponse();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching purchases");
            return new SearchResponse();
        }
    }

    public async Task<bool> DeletePurchaseAsync(Guid purchaseId)
    {
        try
        {
            var response = await _client.DeleteAsync<PurchaseDocument>(purchaseId, d => d
                .Index(PurchasesIndexName)
                .Refresh(Elasticsearch.Net.Refresh.WaitFor)
            );

            if (response.IsValid)
            {
                _logger.LogInformation("Successfully deleted purchase {PurchaseId}", purchaseId);
                return true;
            }
            else
            {
                _logger.LogError("Failed to delete purchase {PurchaseId}: {Error}", 
                    purchaseId, response.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting purchase {PurchaseId}", purchaseId);
            return false;
        }
    }

    public async Task<bool> IndexTransportAsync(TransportDocument transport)
    {
        try
        {
            var response = await _client.IndexAsync(transport, idx => idx
                .Index(TransportsIndexName)
                .Id(transport.Id)
                .Refresh(Elasticsearch.Net.Refresh.WaitFor)
            );

            if (response.IsValid)
            {
                _logger.LogInformation("Successfully indexed transport {TransportId}", transport.Id);
                return true;
            }
            else
            {
                _logger.LogError("Failed to index transport {TransportId}: {Error}", 
                    transport.Id, response.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing transport {TransportId}", transport.Id);
            return false;
        }
    }

    public async Task<SearchResponse> SearchTransportsAsync(Models.SearchRequest request)
    {
        try
        {
            var query = BuildTransportQuery(request);
            
            var searchResponse = await _client.SearchAsync<TransportDocument>(s => s
                .Index(TransportsIndexName)
                .Query(q => query)
                .From((request.Page - 1) * request.PageSize)
                .Size(request.PageSize)
                .Sort(sort => sort.Descending(t => t.AssignedAt))
            );

            if (searchResponse.IsValid)
            {
                return new SearchResponse
                {
                    TransportResults = searchResponse.Documents.ToList(),
                    TotalCount = searchResponse.Total,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
            else
            {
                _logger.LogError("Failed to search transports: {Error}", searchResponse.DebugInformation);
                return new SearchResponse();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching transports");
            return new SearchResponse();
        }
    }

    public async Task<bool> DeleteTransportAsync(Guid transportId)
    {
        try
        {
            var response = await _client.DeleteAsync<TransportDocument>(transportId, d => d
                .Index(TransportsIndexName)
                .Refresh(Elasticsearch.Net.Refresh.WaitFor)
            );

            if (response.IsValid)
            {
                _logger.LogInformation("Successfully deleted transport {TransportId}", transportId);
                return true;
            }
            else
            {
                _logger.LogError("Failed to delete transport {TransportId}: {Error}", 
                    transportId, response.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transport {TransportId}", transportId);
            return false;
        }
    }

    public async Task<SearchResponse> SearchAllAsync(Models.SearchRequest request)
    {
        try
        {
            var response = new SearchResponse
            {
                Page = request.Page,
                PageSize = request.PageSize
            };

            // Search offers
            var offerRequest = new Models.SearchRequest
            {
                SellerId = request.SellerId,
                SearchText = request.SearchText,
                Page = request.Page,
                PageSize = request.PageSize / 3
            };
            var offerResults = await SearchOffersAsync(offerRequest);

            // Search purchases
            var purchaseRequest = new Models.SearchRequest
            {
                BuyerId = request.BuyerId,
                SearchText = request.SearchText,
                Page = request.Page,
                PageSize = request.PageSize / 3
            };
            var purchaseResults = await SearchPurchasesAsync(purchaseRequest);

            // Search transports
            var transportRequest = new Models.SearchRequest
            {
                CarrierId = request.CarrierId,
                SearchText = request.SearchText,
                Page = request.Page,
                PageSize = request.PageSize / 3
            };
            var transportResults = await SearchTransportsAsync(transportRequest);

            response.OfferResults = offerResults.OfferResults;
            response.PurchaseResults = purchaseResults.PurchaseResults;
            response.TransportResults = transportResults.TransportResults;
            response.TotalCount = offerResults.TotalCount + purchaseResults.TotalCount + transportResults.TotalCount;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching all");
            return new SearchResponse();
        }
    }

    private QueryContainer BuildPurchaseQuery(Models.SearchRequest request)
    {
        var mustQueries = new List<QueryContainer>();

        if (request.BuyerId != Guid.Empty)
        {
            mustQueries.Add(new TermQuery
            {
                Field = Infer.Field<PurchaseDocument>(p => p.BuyerId),
                Value = request.BuyerId
            });
        }

        if (request.SellerId != Guid.Empty)
        {
            mustQueries.Add(new TermQuery
            {
                Field = Infer.Field<PurchaseDocument>(p => p.SellerId),
                Value = request.SellerId
            });
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.ToLowerInvariant();
            
            var searchQueries = new QueryContainer[]
            {
                new MatchQuery
                {
                    Field = Infer.Field<PurchaseDocument>(p => p.SearchText),
                    Query = searchText,
                    Boost = 1.0
                },
                new MultiMatchQuery
                {
                    Fields = Infer.Fields<PurchaseDocument>(f => f.Make, f => f.Model),
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

    private QueryContainer BuildTransportQuery(Models.SearchRequest request)
    {
        var mustQueries = new List<QueryContainer>();

        if (request.CarrierId != Guid.Empty)
        {
            mustQueries.Add(new TermQuery
            {
                Field = Infer.Field<TransportDocument>(t => t.CarrierId),
                Value = request.CarrierId
            });
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText = request.SearchText.ToLowerInvariant();
            
            mustQueries.Add(new MatchQuery
            {
                Field = Infer.Field<TransportDocument>(t => t.SearchText),
                Query = searchText
            });
        }

        return new BoolQuery
        {
            Must = mustQueries
        };
    }
}