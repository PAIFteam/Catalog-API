using Catalog.Core.Domain.Entities.RabbitMQ;
using MassTransit;
using Microsoft.Extensions.Logging;
using Catalog.Core.Application.UseCases.GameUser.AddGameUser;
using Catalog.Core.Domain.Interfaces;
using Payments.Core.Domain.Entities.RabbitMQ;

namespace Payments.Core.Entities.RabbitMq
{
    public class PaymentProcessedEventConsumer: IConsumer<PaymentProcessedMessage>
    {
        private readonly IAddGameUserUseCase _addGameUserUseCase;
        private readonly ILogger<PaymentProcessedEventConsumer> _logger;

        public PaymentProcessedEventConsumer(
            IAddGameUserUseCase  addGameUserUse,
            ILogger<PaymentProcessedEventConsumer> logger
            )
        {
            _addGameUserUseCase = addGameUserUse;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<PaymentProcessedMessage> context)
        {

            // Lógica para efetivar o jogo ao usuário se o pagamento foi aprovado
            Console.WriteLine($"Gravando dados para efetivar o jogo ao usuário se o pagamento foi aprovado,  {context.Message.IdUser} " +
                $" IdGame ({context.Message.IdGame}) e Price {context.Message.Price.ToString()}");

            AddGameUserOutput addGameUserOutput = await _addGameUserUseCase.ExecuteAsync(
                new AddGameUserInput(context.Message.IdUser,context.Message.IdGame,context.Message.Price));
            
            await Task.CompletedTask;
        }
    }
}
