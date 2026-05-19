namespace Catalog.Core.Domain.Interfaces
{
    /// <summary>
    /// Interface para UseCase que orquestra a carga de games do SQL Server para Elasticsearch
    /// </summary>
    public interface ILoadGamesElasticsearchUseCase
    {
        /// <summary>
        /// Executa o processo de carga de games do SQL Server para Elasticsearch
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>
        /// Resultado da execução contendo:
        /// - Success: Indicador de sucesso
        /// - Message: Mensagem descritiva do resultado
        /// - GamesLoaded: Quantidade de jogos carregados
        /// </returns>
        Task<LoadGamesResult> ExecuteAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Resultado da operação de carga de games
    /// </summary>
    public class LoadGamesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int GamesLoaded { get; set; }

        public LoadGamesResult(bool success, string message, int gamesLoaded = 0)
        {
            Success = success;
            Message = message;
            GamesLoaded = gamesLoaded;
        }
    }
}
