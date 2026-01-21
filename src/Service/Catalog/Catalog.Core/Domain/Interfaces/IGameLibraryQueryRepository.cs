namespace Catalog.Core.Domain.Interfaces;

public interface IGameLibraryQueryRepository
{
    Task<IReadOnlyList<object>> GetAllGamesAsync();
    Task<decimal?> GetGamePriceAsync(int idGame);
    Task<IReadOnlyList<int>> GetUserGameIdsAsync(int idUser);
    Task<IReadOnlyList<object>> GetUserGamesAsync(int idUser);
}
