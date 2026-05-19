using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catalog.Core.Domain.Interfaces;
using Catalog.Core.Domain.Entities;
using Catalog.Infra.Data.Repositories.Catalog;
using Catalog.Core.Application.UseCases.GameUser.PutGameUser;
using Catalog.Core.Application.UseCases.GameUser.AddGameUser;
using Catalog.Core.Application.UseCases.LoadGamesElasticsearch;
using Nest;

namespace Catalog.Infra.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services)
        {
            //Registro do MediaR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    Assembly.GetExecutingAssembly(),
                    Assembly.GetAssembly(typeof(PutGameUserUseCase))!
                    );
                cfg.RegisterServicesFromAssemblies(
                   Assembly.GetExecutingAssembly(),
                   Assembly.GetAssembly(typeof(AddGameUserUseCase))!
                   );
            });

            //Registro dos Repositorios
            services.AddScoped<IAddGameUserRepository, AddUserRepository>();
            services.AddScoped<IGameLibraryQueryRepository, GameLibraryQueryRepository>();
            services.AddScoped<IGameAdminRepository, GameAdminRepository>();
            services.AddScoped<IGameSqlRepository, GameSqlRepository>();
            services.AddScoped<IGameElasticsearchRepository, GameElasticsearchRepository>();

            //Registro do Elasticsearch
            var elasticsearchUrl = services.BuildServiceProvider().GetRequiredService<IConfiguration>()
                .GetValue<string>("Elasticsearch:Url") ?? "http://localhost:9200";

            var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
                .DefaultMappingFor<GameInfo>(m => m
                    .IndexName("games")
                )
                .DisableDirectStreaming()
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false)
                .EnableApiVersioningHeader(); 

            services.AddSingleton<IElasticClient>(new ElasticClient(settings));

            //Registro dos UseCases
            services.AddScoped<PutGameUserUseCase>();
            services.AddScoped<AddGameUserUseCase>();
            services.AddScoped<PostSalesReportUseCase>();
            services.AddScoped<ILoadGamesElasticsearchUseCase, LoadGamesElasticsearchUseCase>();

            return services;
        }
    }
}
