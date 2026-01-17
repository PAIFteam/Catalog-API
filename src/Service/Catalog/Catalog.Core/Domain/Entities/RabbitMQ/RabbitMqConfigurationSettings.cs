using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Domain.Entities.RabbitMQ
{
    public class RabbitMqConfigurationSettings
    {
        public const string OPTION_KEY = "RabbitSettings";

        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<int> RedeliveryInSeconds { get; set; }
        public List<int> RetryInSeconds { get; set; }
        public string QueueName { get; set; }
        public string QueueNameConsumer { get; set; }
        
        public string ScheduleQueueName { get; set; }
        public bool StartConsumer { get; set; } = false;

        //rabbitmq://[usuário:senha@]host[:porta]/[vhost/][nome-da-fila-ou-exchange]
        //public Uri GetQueueAdress() => new Uri($"amqp://{Username}:{Password}@{HostName}:5672/{QueueName}");
        public Uri GetQueueAdress() => new Uri($"rabbitmq://{Username}:{Password}@{HostName}:5672/{QueueName}");
        public Uri GetQueueAdressConsumer() => new Uri($"rabbitmq://{Username}:{Password}@{HostName}:5672/{QueueNameConsumer}");
    }
}
