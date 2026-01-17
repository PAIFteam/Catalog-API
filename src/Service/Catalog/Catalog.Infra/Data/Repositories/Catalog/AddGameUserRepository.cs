using Catalog.Core.Domain.Entities;
using Catalog.Core.Domain.Entities.Base;
using Catalog.Core.Domain.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Catalog.Infra.Data.Repositories.Catalog
{
    public class AddUserRepository: IAddGameUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<AddUserRepository> _logger;
        public AddUserRepository(IConfiguration confiuration, ILogger<AddUserRepository> logger)
        {
            _connectionString = confiuration.GetConnectionString("DB_SQL_PAIF_GAMES")
                                    ?? throw new InvalidOperationException("Connection string 'DB_SQL_PAIF_GAMES' not found.");

            _logger = logger;
        }

        private IDbConnection CreateConnection()=> new SqlConnection(_connectionString);

        public async Task<OutPutBase> AddGameUserAsync(GameUser gameuser)
        {
            try
            {
                using var connection = CreateConnection();
                string sql = @"
                                declare @new_id_sale integer
                                declare @new_id_sale_item integer

                                declare @id_promotion integer = 1
                                --declare @id_user integer = 3
                                --declare @id_game integer = 1
                                --declare @price decimal(9,2) = 55.63

                                begin 
	                                select @new_id_sale = isnull(max(id_sale),0) + 1 from dbo.sale 
	
	                                INSERT INTO dbo.sale (id_sale, date_sale, id_user, price_total) 
                                                        VALUES (@new_id_sale, getdate(), @IdUser, @Price) 
	
	                                select @new_id_sale_item = isnull(max(id_sale_item),0) + 1 from dbo.sale_item 
	
	                                INSERT INTO dbo.sale_item (id_sale_item, id_sale, id_game, id_promotion, price) 
                                                        VALUES (@new_id_sale_item, @new_id_sale, @IdGame, @id_promotion, @Price) 

                                end
                              ";

                var result = await connection.ExecuteScalarAsync(sql, gameuser);
                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("Falha ao inserir Game User");

                return new OutPutBase();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users count.");
                throw;
            }
        }
        
    }
}
