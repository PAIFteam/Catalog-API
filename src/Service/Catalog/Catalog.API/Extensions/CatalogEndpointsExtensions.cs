using Catalog.Core.Application.UseCases.GameUser.PutGameUser;
using Catalog.Core.Application.Commands;
using Catalog.Core.Application.DTOs;
using Catalog.Core.Application.Queries;
using Catalog.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading;
using Catalog.Core.Application.UseCases.LoadGamesElasticsearch;


namespace Catalog.API.Extensions
{
    public static class CatalogEndpointsExtensions
    {
        public static void MapUserEndpoints(this WebApplication app)
        {

            var api = app.MapGroup("/catalog/api");

            api.MapGet("/games/all", async (IGameLibraryQueryRepository gameLibraryQueryRepository) =>
            {
                var games = await gameLibraryQueryRepository.GetAllGamesAsync();
                return Results.Ok(games);
            })
                .WithName("GetAllGames")
                .WithSummary("Listar todos os games")
                .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
                .RequireAuthorization(policy => policy.RequireRole("User", "Admin"));

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

            api.MapPost("/report/sales", async (
                PostSalesReportUseCase postSalesReportUseCase,
                ILogger<Program> logger) =>
                    {
                        try
                        {
                            var result = await postSalesReportUseCase.ExecuteAsync();

                            if (result == null)
                                return Results.NotFound("Nenhum processo executado com os critérios fornecidos.");

                            return Results.Ok(result);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Um erro ocorreu ao processar a o relatório de vendas.");
                            return Results.BadRequest("Um erro ocorreu ao processar sua solicitação.");
                        }

                    })
                .WithName("ReportSales")
                .WithDescription("Gera relatório de Gestão de Vendas diário")
                .Produces<PutGameUserOutput>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization(policy => policy.RequireRole("User", "Admin"));

            // Elasticsearch Endpoints
            api.MapPost("/elasticsearch/load-games", async (
                ILoadGamesElasticsearchUseCase loadGamesUseCase,
                ILogger<Program> logger) =>
            {
                try
                {
                    logger.LogInformation("Iniciando carga de games para Elasticsearch");
                    var result = await loadGamesUseCase.ExecuteAsync();

                    if (!result.Success)
                    {
                        logger.LogWarning($"Falha na carga de games: {result.Message}");
                        return Results.BadRequest(result);
                    }

                    logger.LogInformation($"Sucesso na carga de games: {result.Message}");
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao carregar games para Elasticsearch");
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            })
                .WithName("LoadGamesElasticsearch")
                .WithSummary("Carregar games do SQL Server para Elasticsearch")
                .WithDescription("Executa a carga de todos os games do SQL Server para o índice 'games' no Elasticsearch")
                .Produces<LoadGamesResult>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            api.MapGet("/elasticsearch/games", async (
                IGameElasticsearchRepository elasticsearchRepository,
                ILogger<Program> logger) =>
            {
                try
                {
                    logger.LogInformation("Buscando todos os games no Elasticsearch");
                    var games = await elasticsearchRepository.GetAllGamesAsync();
                    return Results.Ok(games);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao buscar games no Elasticsearch");
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            })
                .WithName("GetGamesElasticsearch")
                .WithSummary("Listar todos os games do Elasticsearch")
                .WithDescription("Retorna todos os games armazenados no índice 'games' do Elasticsearch")
                .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization(policy => policy.RequireRole("User", "Admin"));

            api.MapGet("/elasticsearch/games/{gameId:int}", async (
                int gameId,
                IGameElasticsearchRepository elasticsearchRepository,
                ILogger<Program> logger) =>
            {
                try
                {
                    logger.LogInformation($"Buscando game {gameId} no Elasticsearch");
                    var game = await elasticsearchRepository.GetGameByIdAsync(gameId);

                    if (game == null)
                        return Results.NotFound($"Game com ID {gameId} não encontrado");

                    return Results.Ok(game);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Erro ao buscar game {gameId} no Elasticsearch");
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            })
                .WithName("GetGameByIdElasticsearch")
                .WithSummary("Buscar game por ID no Elasticsearch")
                .WithDescription("Retorna um game específico armazenado no Elasticsearch pelo seu ID")
                .Produces<object>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization(policy => policy.RequireRole("User", "Admin"));

            api.MapGet("/elasticsearch/search", async (
                [FromQuery] string q,
                IGameElasticsearchRepository elasticsearchRepository,
                ILogger<Program> logger) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(q))
                        return Results.BadRequest("Parâmetro de busca 'q' é obrigatório");

                    logger.LogInformation($"Buscando games com termo: {q}");
                    var games = await elasticsearchRepository.SearchGamesAsync(q);
                    return Results.Ok(games);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Erro ao buscar games com termo '{q}' no Elasticsearch");
                    return Results.StatusCode(StatusCodes.Status500InternalServerError);
                }
            })
                .WithName("SearchGamesElasticsearch")
                .WithSummary("Buscar games no Elasticsearch")
                .WithDescription("Realiza busca de games por termo (nome, empresa ou tipo de jogo)")
                .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireAuthorization(policy => policy.RequireRole("User", "Admin"));
        }
    }
}