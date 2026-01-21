using Catalog.Core.Application.DTOs;
using Catalog.Core.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalog.Infra.Data.Repositories.Catalog;

public sealed class GameAdminRepository : IGameAdminRepository
{
    private readonly string _connectionString;
    private readonly ILogger<GameAdminRepository> _logger;

    public GameAdminRepository(IConfiguration configuration, ILogger<GameAdminRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DB_SQL_PAIF_GAMES")
                            ?? throw new InvalidOperationException("Connection string 'DB_SQL_PAIF_GAMES' not found.");
        _logger = logger;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<bool> RegisterAsync(string name, decimal price)
    {
        const string sql = @"
            declare @new_id_game int;

            select @new_id_game = isnull(max(id_game), 0) + 1 from dbo.games;

            insert into dbo.games (id_game, name, id_company, id_type_game, price)
            values (@new_id_game, @Name, 1, 1, @Price);";

        try
        {
            using var connection = CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { Name = name, Price = price });
            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering game");
            throw;
        }
    }

    public async Task<GameDto?> GetByIdAsync(int idGame)
    {
        const string sql = @"select top 1 id_game as IdGame, name as Name, price as Price from dbo.games where id_game = @IdGame";

        try
        {
            using var connection = CreateConnection();
            var game = await connection.QuerySingleOrDefaultAsync<GameDto>(sql, new { IdGame = idGame });
            if (game is null) return null;
            return game;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game by id {IdGame}", idGame);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int idGame, string name, decimal price)
    {
        const string sql = @"update dbo.games set name = @Name, price = @Price where id_game = @IdGame";

        try
        {
            using var connection = CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { IdGame = idGame, Name = name, Price = price });
            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating game {IdGame}", idGame);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int idGame)
    {
        const string sql = @"delete from dbo.games where id_game = @IdGame";

        try
        {
            using var connection = CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { IdGame = idGame });
            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting game {IdGame}", idGame);
            throw;
        }
    }
}
