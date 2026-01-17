using Catalog.Core.Application.UseCases.GameUser.PutGameUser;
using Catalog.Core.Domain;
using Catalog.Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Catalog.API.Extensions
{
    public static class UserEndpointsExtensions
    {
        public static void MapUserEndpoints(this WebApplication app)
        {

            app.MapGet("/PutGameUser", async (int idUser,
            int idGame,
            decimal price,
            PutGameUserUseCase putGameUserUseCase,
            ILogger<Program> logger) =>
            {
                try
                {
                    var input = new PutGameUserInput(idUser, idGame, price);

                    var result = await putGameUserUseCase.ExecuteAsync(input);

                    if (result == null)
                        return Results.NotFound("Nenhum usuário encontrado com os critérios fornecidos.");

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Um erro ocorreu ao processar a solicitação PutGameUser.");
                    return Results.BadRequest("Um erro ocorreu ao processar sua solicitação.");
                }

            })
               .WithName("GetGameUser")
               .WithDescription("Recebe uma requisição para adicionar um jogo à \r\nbiblioteca de um usuário ")
               .Produces<IEnumerable<User>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status400BadRequest);
            //.Produces(StatusCodes.Status401Unauthorized)
            //.RequireAuthorization(policy => policy.RequireRole("Admin"));

        }  
    }
}