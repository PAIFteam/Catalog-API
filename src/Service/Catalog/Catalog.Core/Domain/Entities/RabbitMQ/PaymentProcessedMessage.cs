using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Core.Domain.Entities.RabbitMQ
{
    public class PaymentProcessedMessage
    {
        public PaymentProcessedMessage(int idUser, int idGame, decimal price, bool aproved, string message)
        {
            IdUser = idUser;
            IdGame = idGame;
            Price = price;
            Aproved = aproved;
            Message = message;


        }
        public int IdUser { get; set; }
        public int IdGame { get; set; }
        public decimal Price { get; set; }
        public bool Aproved { get; set; }
        public string Message { get; set; }


    }
}
