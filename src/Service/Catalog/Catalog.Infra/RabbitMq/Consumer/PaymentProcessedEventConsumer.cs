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
        private readonly AddGameUserUseCase _addGameUserUseCase;
        private readonly ILogger<PaymentProcessedEventConsumer> _logger;

        public PaymentProcessedEventConsumer(
            AddGameUserUseCase addGameUserUse,
            ILogger<PaymentProcessedEventConsumer> logger
            )
        {
            _addGameUserUseCase = addGameUserUse;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<PaymentProcessedMessage> context)
        {
            _logger.LogInformation($"Consumer do PaymentProcessedMessage - Gravando dados para efetivar o jogo ao usuário se o pagamento foi aprovado,  {context.Message.IdUser} " +
                $" IdGame ({context.Message.IdGame}) e Price {context.Message.Price.ToString()}");
            if (!context.Message.Aproved)
            {
                _logger.LogInformation("Pagamento recusado para o usuário {IdUser} e jogo {IdGame}. Nenhuma concessão será executada.",
                    context.Message.IdUser,
                    context.Message.IdGame);
                return;
            }

            AddGameUserOutput addGameUserOutput = await _addGameUserUseCase.ExecuteAsync(
                new AddGameUserInput(context.Message.IdUser,context.Message.IdGame,context.Message.Price));

            if (!addGameUserOutput.Result)
            {
                _logger.LogWarning("Falha ao conceder jogo {IdGame} ao usuário {IdUser}: {Message}",
                    context.Message.IdGame,
                    context.Message.IdUser,
                    addGameUserOutput.Message);
            }
        }
    }
}
