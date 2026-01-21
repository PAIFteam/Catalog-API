using Catalog.Core.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalog.Infra.Data.Repositories.Catalog;

public class GameLibraryQueryRepository : IGameLibraryQueryRepository
{
    private readonly string _connectionString;
    private readonly ILogger<GameLibraryQueryRepository> _logger;

    public GameLibraryQueryRepository(IConfiguration configuration, ILogger<GameLibraryQueryRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DB_SQL_PAIF_GAMES")
                            ?? throw new InvalidOperationException("Connection string 'DB_SQL_PAIF_GAMES' not found.");
        _logger = logger;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IReadOnlyList<object>> GetAllGamesAsync()
    {
        const string sql = @"select id_game as IdGame, name as Name, price as Price from dbo.games order by id_game";

        try
        {
            using var connection = CreateConnection();
            var result = await connection.QueryAsync(sql);
            return result.AsList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying all games");
            throw;
        }

    }

    public async Task<IReadOnlyList<object>> GetUserGamesAsync(int idUser)
    {
        const string sql = @"SELECT DISTINCT
                                           g.id_game as IdGame,
                                           g.name as Name
                                    FROM dbo.sale s
                                    INNER JOIN dbo.sale_item si ON si.id_sale = s.id_sale
                                    INNER JOIN dbo.games g ON g.id_game = si.id_game
                                    WHERE s.id_user = @IdUser";

        try
        {
            using var connection = CreateConnection();
            var result = await connection.QueryAsync(sql, new { IdUser = idUser });
            return result.AsList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying game details for id_user={IdUser}", idUser);
            throw;
        }
    }

    public async Task<decimal?> GetGamePriceAsync(int idGame)
    {
        const string sql = "select top 1 price from dbo.games where id_game = @IdGame";

        try
        {
            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<decimal?>(sql, new { IdGame = idGame });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying game price for id_game={IdGame}", idGame);
            throw;
        }
    }

    public async Task<IReadOnlyList<int>> GetUserGameIdsAsync(int idUser)
    {
        const string sql = @"
            select distinct si.id_game
            from dbo.sale s
            inner join dbo.sale_item si on si.id_sale = s.id_sale
            where s.id_user = @IdUser
            order by si.id_game";

        try
        {
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<int>(sql, new { IdUser = idUser });
            return result.AsList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying games for id_user={IdUser}", idUser);
            throw;
        }
    }
}
