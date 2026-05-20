using Catalog.Core.Domain.Entities;
using Catalog.Core.Domain.Interfaces;
using Nest;
using Microsoft.Extensions.Logging;

namespace Catalog.Infra.Data.Repositories.Catalog;

public class GameElasticsearchRepository : IGameElasticsearchRepository
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<GameElasticsearchRepository> _logger;
    private const string IndexName = "games";

    public GameElasticsearchRepository(IElasticClient elasticClient, ILogger<GameElasticsearchRepository> logger)
    {
        _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        _logger = logger;
    }

    public async Task<bool> LoadGamesAsync(IEnumerable<GameInfo> games, CancellationToken cancellationToken = default)
    {
        try
        {
            var gamesList = games.ToList();

            if (!gamesList.Any())
            {
                _logger.LogWarning("No games provided to load");
                return false;
            }

            // Verificar se o índice existe, se não, criar
            var indexExists = await _elasticClient.Indices.ExistsAsync(IndexName, ct: cancellationToken);

            if (!indexExists.Exists)
            {
                _logger.LogInformation($"Creating Elasticsearch index: {IndexName}");

                var createIndexResponse = await _elasticClient.Indices.CreateAsync(
                    IndexName,
                    c => c.Map<GameInfo>(m => m
                        .AutoMap()
                        .Properties(p => p
                            .Keyword(k => k
                                .Name(n => n.Name)
                            )
                            .Keyword(k => k
                                .Name(n => n.NameCompany)
                            )
                            .Keyword(k => k
                                .Name(n => n.NameTypeGame)
                            )
                        )
                    ),
                    ct: cancellationToken
                );

                if (!createIndexResponse.IsValid)
                {
                    _logger.LogError($"Failed to create index: {createIndexResponse.ServerError?.Error?.Reason}");
                    return false;
                }
            }

            // Inserir os games
            var bulkRequest = new BulkRequest(IndexName)
            {
                Operations = new BulkOperationsCollection<IBulkOperation>()
            };

            foreach (var game in gamesList)
            {
                bulkRequest.Operations.Add(
                    new BulkIndexOperation<GameInfo>(game) { Id = game.IdGame.ToString() }
                );
            }

            var response = await _elasticClient.BulkAsync(bulkRequest, cancellationToken);

            if (response.IsValid)
            {
                _logger.LogInformation($"Successfully loaded {gamesList.Count} games into Elasticsearch index '{IndexName}'");
                return true;
            }
            else
            {
                // Verificar se há erros reais (não apenas vazios)
                var itemsWithRealErrors = response.ItemsWithErrors
                    .Where(x => !string.IsNullOrEmpty(x.Error?.Reason))
                    .ToList();

                if (itemsWithRealErrors.Any())
                {
                    _logger.LogError($"Failed to bulk insert games: {response.ServerError?.Error?.Reason}");
                    foreach (var item in itemsWithRealErrors)
                    {
                        _logger.LogError($"Error on item {item.Id}: {item.Error?.Reason}");
                    }
                    return false;
                }

                // Se todos os erros são vazios, considerou sucesso
                _logger.LogInformation($"Successfully loaded {gamesList.Count} games into Elasticsearch index '{IndexName}'");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading games into Elasticsearch");
            throw;
        }
    }

    public async Task<IEnumerable<GameInfo>> SearchGamesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _elasticClient.SearchAsync<GameInfo>(
                s => s
                    .Index(IndexName)
                    .Query(q => q
                        .MultiMatch(m => m
                            .Query(searchTerm)
                            .Fields(f => f
                                .Field(g => g.Name)
                                .Field(g => g.NameCompany)
                                .Field(g => g.NameTypeGame)
                            )
                        )
                    )
                    .Size(1000),
                cancellationToken
            );

            if (!response.IsValid)
            {
                _logger.LogWarning($"Search query failed: {response.ServerError?.Error?.Reason}");
                return Enumerable.Empty<GameInfo>();
            }

            return response.Documents.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching games in Elasticsearch");
            throw;
        }
    }

    public async Task<IEnumerable<GameInfo>> GetAllGamesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _elasticClient.SearchAsync<GameInfo>(
                s => s
                    .Index(IndexName)
                    .Query(q => q.MatchAll())
                    .Size(10000),
                cancellationToken
            );

            if (!response.IsValid)
            {
                _logger.LogWarning($"Get all query failed: {response.ServerError?.Error?.Reason}");
                return Enumerable.Empty<GameInfo>();
            }

            _logger.LogInformation($"Retrieved {response.Documents.Count} games from Elasticsearch");
            return response.Documents.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all games from Elasticsearch");
            throw;
        }
    }

    public async Task<GameInfo?> GetGameByIdAsync(int gameId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _elasticClient.GetAsync<GameInfo>(gameId.ToString(), g => g.Index(IndexName), cancellationToken);

            if (!response.IsValid || !response.Found)
            {
                _logger.LogWarning($"Game with ID {gameId} not found in Elasticsearch");
                return null;
            }

            return response.Source;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving game {gameId} from Elasticsearch");
            throw;
        }
    }
}
