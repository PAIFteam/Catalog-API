using Catalog.Core.Application.UseCases.GameUser.AddGameUser;
using Catalog.Core.Domain.Interfaces;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Payments.Core.Domain.Entities.RabbitMQ;
using Payments.Core.Entities.RabbitMq;

namespace Catalog.UnitTests;

public class PaymentProcessedEventConsumerTests
{
    [Fact]
    public async Task Consume_PagamentoRecusado_NaoDeveExecutarConcessaoDoJogo()
    {
        var repository = new Mock<IAddGameUserRepository>(MockBehavior.Strict);
        var addGameUserUseCase = new AddGameUserUseCase(repository.Object, Mock.Of<ILogger<AddGameUserUseCase>>());
        var context = new Mock<ConsumeContext<PaymentProcessedMessage>>();
        context.SetupGet(x => x.Message).Returns(new PaymentProcessedMessage(1, 2, 10m, false, "recusado"));
        var sut = new PaymentProcessedEventConsumer(addGameUserUseCase, Mock.Of<ILogger<PaymentProcessedEventConsumer>>());

        await sut.Consume(context.Object);

        repository.Verify(x => x.AddGameUserAsync(It.IsAny<Catalog.Core.Domain.Entities.GameUser>()), Times.Never);
    }
}
