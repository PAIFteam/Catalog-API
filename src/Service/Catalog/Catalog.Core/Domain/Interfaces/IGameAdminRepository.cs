using Catalog.Core.Application.DTOs;

namespace Catalog.Core.Domain.Interfaces;

public interface IGameAdminRepository
{
    Task<bool> RegisterAsync(string name, decimal price);
    Task<GameDto?> GetByIdAsync(int idGame);
    Task<bool> UpdateAsync(int idGame, string name, decimal price);
    Task<bool> DeleteAsync(int idGame);
}
