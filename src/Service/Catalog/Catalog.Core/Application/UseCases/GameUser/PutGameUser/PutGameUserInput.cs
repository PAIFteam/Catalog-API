using Catalog.Core.Domain;
using Catalog.Core.Domain.Entities;

namespace Catalog.Core.Application.UseCases.GameUser.PutGameUser
{
    public class PutGameUserInput
    {
        public PutGameUserInput(int idUser, int idGame, decimal price)
        {
            IdUser = idUser;
            IdGame = idGame;
            Price = price;

        }
        public int IdUser { get; set; }
        public int IdGame { get; set; }
        public decimal Price{ get; set; }

    }
}
