using Catalog.Core.Application.UseCases.GameUser.PutGameUser;
using Catalog.Core.Application.Commands;
using Catalog.Core.Application.DTOs;
using Catalog.Core.Application.Queries;
using Catalog.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading;


namespace Catalog.API.Extensions
{
    public static class CatalogEndpointsExtensions
    {
        public static void MapUserEndpoints(this WebApplication app)
        {

            var api = app.MapGroup("/api");

            api.MapGet("/games/all", async (IGameLibraryQueryRepository gameLibraryQueryRepository) =>
            {
                var games = await gameLibraryQueryRepository.GetAllGamesAsync();
                return Results.Ok(games);
            })
                .WithName("GetAllGames")
                .WithSummary("Listar todos os games")
                .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("User"));

            api.MapGet("/games/me", async (
                HttpContext httpContext,
                IGameLibraryQueryRepository gameLibraryQueryRepository) =>
            {
                if (!httpContext.User.TryGetUserId(out var idUser))
                    return Results.Unauthorized();

                var games = await gameLibraryQueryRepository.GetUserGamesAsync(idUser);
                return Results.Ok(games);
            })
                .WithName("GetMyGames")
                .WithSummary("Listar meus games")
                .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization(policy => policy.RequireRole("User", "Admin"));

            api.MapPost("/game/buy/{id:int}", async (
            int id,
            HttpContext httpContext,
            IGameLibraryQueryRepository gameLibraryQueryRepository,
            PutGameUserUseCase putGameUserUseCase,
            ILogger<Program> logger) =>
            {
                try
                {
                    if (!httpContext.User.TryGetUserId(out var idUser))
                        return Results.Unauthorized();

                    var price = await gameLibraryQueryRepository.GetGamePriceAsync(id);
                    if (price is null)
                        return Results.NotFound("Game não encontrado.");

                    var input = new PutGameUserInput(idUser, id, price.Value);

                    var result = await putGameUserUseCase.ExecuteAsync(input);

                    if (result == null)
                        return Results.NotFound("Nenhum usuário encontrado com os critérios fornecidos.");

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Um erro ocorreu ao processar a solicitação de compra de game.");
                    return Results.BadRequest("Um erro ocorreu ao processar sua solicitação.");
                }

            })
               .WithName("BuyGame")
               .WithDescription("Inicia a compra de um jogo para o usuário autenticado (id via JWT).")
               .Produces<PutGameUserOutput>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status401Unauthorized)
               .RequireAuthorization(policy => policy.RequireRole("User", "Admin"));


            api.MapPost("/game/register", async (
                RegisterGameCommand command,
                MediatR.ISender sender) =>
            {
                var ok = await sender.Send(command);
                return ok ? Results.Ok() : Results.BadRequest();
            })
                .WithName("RegisterGame")
                .WithSummary("Registrar jogo")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapGet("/game/{idGame:int}", async (
                int idGame,
                MediatR.ISender sender) =>
            {
                var game = await sender.Send(new GetGameByUIdQuery(idGame));
                return game is null ? Results.NotFound() : Results.Ok(game);
            })
                .WithName("GetGameById")
                .WithSummary("Buscar jogo por Id")
                .Produces<GameDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapPut("/game/update", async (
                UpdateGameCommand command,
                MediatR.ISender sender) =>
            {
                var ok = await sender.Send(command);
                return ok ? Results.Ok() : Results.NotFound();
            })
                .WithName("UpdateGame")
                .WithSummary("Atualizar jogo")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapDelete("/game/{idGame:int}", async (
                int idGame,
                MediatR.ISender sender) =>
            {
                var ok = await sender.Send(new DeleteGameCommand(idGame));
                return ok ? Results.Ok() : Results.NotFound();
            })
                .WithName("DeleteGame")
                .WithSummary("Remover jogo")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));
        }  
    }
}