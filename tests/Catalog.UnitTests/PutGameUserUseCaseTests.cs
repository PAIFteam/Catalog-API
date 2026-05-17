using Catalog.Core.Application.UseCases.GameUser.PutGameUser;
using Catalog.Core.Domain.Entities.RabbitMQ;
using Catalog.Core.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.UnitTests;

public class PutGameUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_EntradaValida_DevePublicarOrderPlacedMessage()
    {
        var publisher = new Mock<IPublisher>();
        OrderPlacedMessage? published = null;
        var settings = new RabbitMqConfigurationSettings
        {
            HostName = "localhost",
            Username = "guest",
            Password = "guest",
            QueueName = "order_placed_queue",
            QueueNameConsumer = "payment_processed_queue",
            RedeliveryInSeconds = [],
            RetryInSeconds = []
        };
        publisher.Setup(x => x.Publish(It.IsAny<OrderPlacedMessage>(), It.IsAny<Uri>()))
            .Callback<object, Uri>((msg, _) => published = (OrderPlacedMessage)msg)
            .Returns(Task.CompletedTask);

        var sut = new PutGameUserUseCase(settings, publisher.Object, Mock.Of<ILogger<PutGameUserUseCase>>());

        var result = await sut.ExecuteAsync(new PutGameUserInput(10, 20, 99m));

        result.Message.Should().Contain("aguardando pagamento");
        published.Should().NotBeNull();
        published!.IdUser.Should().Be(10);
        published.IdGame.Should().Be(20);
        published.Price.Should().Be(99m);
    }
}
