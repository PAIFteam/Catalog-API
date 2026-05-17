using Catalog.Core.Application.UseCases.GameUser.AddGameUser;
using Catalog.Core.Domain.Entities;
using Catalog.Core.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.UnitTests;

public class AddGameUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_EntradaValida_DevePersistirVinculoJogoUsuario()
    {
        var repository = new Mock<IAddGameUserRepository>();
        GameUser? persisted = null;
        repository.Setup(x => x.AddGameUserAsync(It.IsAny<GameUser>()))
            .Callback<GameUser>(entity => persisted = entity)
            .ReturnsAsync(true);

        var sut = new AddGameUserUseCase(repository.Object, Mock.Of<ILogger<AddGameUserUseCase>>());

        var result = await sut.ExecuteAsync(new AddGameUserInput(1, 2, 50m));

        result.Result.Should().BeTrue();
        persisted.Should().NotBeNull();
        persisted!.IdUser.Should().Be(1);
        persisted.IdGame.Should().Be(2);
    }
}
