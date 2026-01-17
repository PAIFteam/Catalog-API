using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Domain.Entities.RabbitMQ
{
    public class OrderPlacedMessage
    {
        public OrderPlacedMessage(int idUser, int idGame, decimal price)
        {
            IdUser = idUser;
            IdGame = idGame;
            Price = price;

        }
        public int IdUser { get; set; }
        public int IdGame { get; set; }
        public decimal Price { get; set; }

    }
}
