
using Catalog.Core.Domain.Entities.RabbitMQ;
using Catalog.Core.Domain.Interfaces;
using MassTransit;
using MassTransit.MultiBus;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;
using Catalog.Infra.RabbitMq.Publishers;

namespace Catalog.API.Extensions
{
    public static class RabbitMqSettings
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitSettings = new RabbitMqConfigurationSettings();

            configuration
                .GetSection(RabbitMqConfigurationSettings.OPTION_KEY)
                .Bind(rabbitSettings);
            services.AddScoped<IPublisher, Publisher>();
            services.AddScoped(_ => rabbitSettings);

            services.AddMassTransit<IBus>(_ =>
            {
                _.AddBus(context => Bus.Factory.CreateUsingRabbitMq(configure =>
                {
                    //var rabbitUri = new Uri($"ampq://{rabbitSettings.Username}:{rabbitSettings.Password}@{rabbitSettings.HostName}:5672");
                    var rabbitUri = new  Uri($"rabbitmq://{rabbitSettings.Username}:{rabbitSettings.Password}@{rabbitSettings.HostName}:5672"); ///{rabbitSettings.QueueName}
                    configure.Host(rabbitUri, h =>
                    {
                    });

                    //configure.Host("localhost", "/", h => // O primeiro parâmetro é o hostname, o segundo é o virtual host (padrão "/")
                    //{
                    //    h.Username("guest"); // Usuário padrão
                    //    h.Password("guest"); // Senha padrão
                    //});

                    configure.Durable = true;
                    //configure.UseCircuitBreaker(CorrelatedBy =>
                    //{
                    //    CorrelatedBy.TrackingPeriod = TimeSpan.FromMinutes(1);
                    //    CorrelatedBy.TripThreshold = 15;
                    //    CorrelatedBy.ActiveThreshold = 10;
                    //    CorrelatedBy.ResetInterval = TimeSpan.FromMinutes(5);
                    //});

                    //configure.UseInMemoryScheduler(rabbitSettings.ScheduleQueueName);

                    //configure.ReceiveEndpoint(rabbitSettings.QueueName, configureEndpoint =>
                    //{
                    //    var redeliveryIntervals = GetIntervals(rabbitSettings.RedeliveryInSeconds);
                    //    var retryIntervals = GetIntervals(rabbitSettings.RetryInSeconds);

                    //    if (!Equals(redeliveryIntervals, null) && redeliveryIntervals.Any())
                    //    {
                    //        configureEndpoint.UseScheduledRedelivery(r => r.Intervals(redeliveryIntervals));
                    //    }

                    //    if (!Equals(redeliveryIntervals, null) && retryIntervals.Any())
                    //    {
                    //        configureEndpoint.UseMessageRetry(r => r.Intervals(retryIntervals));

                    //    }

                    //});

                    //configure.useHealthCheck(context);
                }));
            });

            return services;
        }
        private static TimeSpan[] GetIntervals(List<int> intervals)
        {
            if (Equals(intervals, null))
            {
                return new TimeSpan[0];
            }

            var nonZeroIntervals = intervals.Where(interval => !Equals(interval, 0));

            return nonZeroIntervals.Select(interval => TimeSpan.FromSeconds(interval)).ToArray();
        }
    }
}
