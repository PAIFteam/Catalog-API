using Catalog.Core.Application.Commands;
using Catalog.Core.Application.DTOs;
using Catalog.Core.Application.Handlers;
using Catalog.Core.Application.Queries;
using Catalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Catalog.UnitTests;

public class GameHandlersTests
{
    [Fact]
    public async Task RegisterGameCommandHandler_DeveDelegarParaRepositorio()
    {
        var repository = new Mock<IGameAdminRepository>();
        repository.Setup(x => x.RegisterAsync("Game", 10m)).ReturnsAsync(true);
        var sut = new RegisterGameCommandHandler(repository.Object);

        var result = await sut.Handle(new RegisterGameCommand { Name = "Game", Price = 10m }, default);

        result.Should().BeTrue();
        repository.Verify(x => x.RegisterAsync("Game", 10m), Times.Once);
    }

    [Fact]
    public async Task GetGameByUIdQueryHandler_DeveRetornarGameDto()
    {
        var repository = new Mock<IGameAdminRepository>();
        repository.Setup(x => x.GetByIdAsync(7)).ReturnsAsync(new GameDto { IdGame = 7, Name = "Game", Price = 99m });
        var sut = new GetGameByUIdQueryHandler(repository.Object);

        var result = await sut.Handle(new GetGameByUIdQuery(7), default);

        result.Should().NotBeNull();
        result!.IdGame.Should().Be(7);
    }

    [Fact]
    public async Task UpdateGameCommandHandler_DeveDelegarParaRepositorio()
    {
        var repository = new Mock<IGameAdminRepository>();
        repository.Setup(x => x.UpdateAsync(7, "Novo", 20m)).ReturnsAsync(true);
        var sut = new UpdateGameCommandHandler(repository.Object);

        var result = await sut.Handle(new UpdateGameCommand { IdGame = 7, Name = "Novo", Price = 20m }, default);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteGameCommandHandler_DeveDelegarParaRepositorio()
    {
        var repository = new Mock<IGameAdminRepository>();
        repository.Setup(x => x.DeleteAsync(7)).ReturnsAsync(true);
        var sut = new DeleteGameCommandHandler(repository.Object);

        var result = await sut.Handle(new DeleteGameCommand(7), default);

        result.Should().BeTrue();
    }
}
