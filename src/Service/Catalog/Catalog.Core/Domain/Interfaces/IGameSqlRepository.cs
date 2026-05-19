using Catalog.Core.Domain.Entities;

namespace Catalog.Core.Domain.Interfaces
{
    /// <summary>
    /// Interface para repositório que recupera dados de games do SQL Server
    /// </summary>
    public interface IGameSqlRepository
    {
        /// <summary>
        /// Recupera todos os games do SQL Server com suas informações de empresa e tipo
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista completa de games com informações relacionadas</returns>
        Task<IEnumerable<GameInfo>> GetAllGamesWithRelationsAsync(CancellationToken cancellationToken = default);
    }
}
