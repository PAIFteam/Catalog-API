using Catalog.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalog.Core.Application.UseCases.LoadGamesElasticsearch;

public class LoadGamesElasticsearchUseCase : ILoadGamesElasticsearchUseCase
{
    private readonly IGameSqlRepository _gameSqlRepository;
    private readonly IGameElasticsearchRepository _gameElasticsearchRepository;
    private readonly ILogger<LoadGamesElasticsearchUseCase> _logger;

    public LoadGamesElasticsearchUseCase(
        IGameSqlRepository gameSqlRepository,
        IGameElasticsearchRepository gameElasticsearchRepository,
        ILogger<LoadGamesElasticsearchUseCase> logger)
    {
        _gameSqlRepository = gameSqlRepository ?? throw new ArgumentNullException(nameof(gameSqlRepository));
        _gameElasticsearchRepository = gameElasticsearchRepository ?? throw new ArgumentNullException(nameof(gameElasticsearchRepository));
        _logger = logger;
    }

    public async Task<LoadGamesResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting load games to Elasticsearch process");

            // Passo 1: Recuperar dados do SQL Server
            _logger.LogInformation("Fetching games from SQL Server");
            var games = await _gameSqlRepository.GetAllGamesWithRelationsAsync(cancellationToken);

            var gamesList = games.ToList();

            if (!gamesList.Any())
            {
                _logger.LogWarning("No games found in SQL Server");
                return new LoadGamesResult(false, "No games found in SQL Server to load", 0);
            }

            _logger.LogInformation($"Retrieved {gamesList.Count} games from SQL Server");

            // Passo 2: Carregar dados no Elasticsearch
            _logger.LogInformation($"Loading {gamesList.Count} games into Elasticsearch");
            var success = await _gameElasticsearchRepository.LoadGamesAsync(gamesList, cancellationToken);

            if (!success)
            {
                _logger.LogError("Failed to load games into Elasticsearch");
                return new LoadGamesResult(false, "Failed to load games into Elasticsearch", 0);
            }

            _logger.LogInformation($"Successfully loaded {gamesList.Count} games into Elasticsearch");
            return new LoadGamesResult(true, $"Successfully loaded {gamesList.Count} games into Elasticsearch", gamesList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during load games to Elasticsearch process");
            return new LoadGamesResult(false, $"Error: {ex.Message}", 0);
        }
    }
}
