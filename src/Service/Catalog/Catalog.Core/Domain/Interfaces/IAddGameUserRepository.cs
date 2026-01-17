using Catalog.Core.Domain.Entities;
using Catalog.Core.Domain.Entities.Base;

namespace Catalog.Core.Domain.Interfaces
{
    public interface IAddGameUserRepository
    {
        Task<OutPutBase> AddGameUserAsync(GameUser gameUser);
        
    }
}
