using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catalog.Core.Domain.Interfaces;
using Catalog.Infra.Data.Repositories.Catalog;
using Catalog.Core.Application.UseCases.GameUser.PutGameUser;
using Catalog.Core.Application.UseCases.GameUser.AddGameUser;

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

            //Registro dos UseCases
            services.AddScoped<PutGameUserUseCase>();
            services.AddScoped<AddGameUserUseCase>();

            return services;
        }
    }
}
