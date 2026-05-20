using Catalog.Core.Domain.Entities;

namespace Catalog.Core.Domain.Interfaces
{
    /// <summary>
    /// Interface para repositório que gerencia operações com Elasticsearch
    /// </summary>
    public interface IGameElasticsearchRepository
    {
        /// <summary>
        /// Carrega todos os games no índice do Elasticsearch
        /// </summary>
        /// <param name="games">Lista de jogos a serem carregados</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se carregado com sucesso, False caso contrário</returns>
        Task<bool> LoadGamesAsync(IEnumerable<GameInfo> games, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca jogos no Elasticsearch por termo de pesquisa
        /// </summary>
        /// <param name="searchTerm">Termo a ser pesquisado (nome, empresa ou tipo de jogo)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de jogos encontrados</returns>
        Task<IEnumerable<GameInfo>> SearchGamesAsync(string searchTerm, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retorna todos os jogos armazenados no Elasticsearch
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de todos os jogos</returns>
        Task<IEnumerable<GameInfo>> GetAllGamesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca um jogo específico pelo ID
        /// </summary>
        /// <param name="gameId">ID do jogo</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Informações do jogo se encontrado, null caso contrário</returns>
        Task<GameInfo?> GetGameByIdAsync(int gameId, CancellationToken cancellationToken = default);
    }
}
