using Catalog.Core.Domain.Entities;
using Catalog.Core.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalog.Infra.Data.Repositories.Catalog;

public class GameSqlRepository : IGameSqlRepository
{
    private readonly string _connectionString;
    private readonly ILogger<GameSqlRepository> _logger;

    public GameSqlRepository(IConfiguration configuration, ILogger<GameSqlRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DB_SQL_PAIF_GAMES")
                            ?? throw new InvalidOperationException("Connection string 'DB_SQL_PAIF_GAMES' not found.");
        _logger = logger;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<GameInfo>> GetAllGamesWithRelationsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                g.id_game AS IdGame,
                g.name AS Name,
                g.price AS Price,
                c.id_company AS IdCompany,
                c.name AS NameCompany,
                tg.id_type_game AS IdTypeGame,
                tg.name AS NameTypeGame
            FROM dbo.games g
            INNER JOIN dbo.company c ON g.id_company = c.id_company
            INNER JOIN dbo.type_game tg ON tg.id_type_game = g.id_type_game
            ORDER BY g.id_game";

        try
        {
            using var connection = CreateConnection();

            var result = await connection.QueryAsync<GameInfo>(sql);

            _logger.LogInformation($"Successfully retrieved {result.Count()} games from SQL Server");

            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving games with relations from SQL Server");
            throw;
        }
    }
}
