using Catalog.Core.Domain.Entities;
using Catalog.Core.Domain.Entities.Base;

namespace Catalog.Core.Domain.Interfaces
{
    public interface IAddGameUserRepository
    {
        Task<bool> AddGameUserAsync(GameUser gameUser);
        
    }
}
