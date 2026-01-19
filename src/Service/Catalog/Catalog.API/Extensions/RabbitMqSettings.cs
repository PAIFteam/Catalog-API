
using Catalog.Core.Domain.Entities.RabbitMQ;
using Catalog.Core.Domain.Interfaces;
using Catalog.Infra.RabbitMq.Publishers;
using GreenPipes;
using MassTransit;
using MassTransit.MultiBus;
using Microsoft.Extensions.DependencyInjection;
using Payments.Core.Entities.RabbitMq;

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
            if (rabbitSettings.StartConsumer)
            {
                CreateBus<IBus>(services, rabbitSettings);
                services.AddMassTransitHostedService();
            }

            return services;
        }
        private static void CreateBus<T>(IServiceCollection services, RabbitMqConfigurationSettings rabbitSettings) where T : class, IBus
        {
            services.AddMassTransit<IBus>(_ =>
            {
                _.AddConsumer<PaymentProcessedEventConsumer>();


                _.UsingRabbitMq((context, configure) =>
                {
                    var rabbitUri = new Uri($"rabbitmq://{rabbitSettings.Username}:{rabbitSettings.Password}@{rabbitSettings.HostName}:5672"); ///{rabbitSettings.QueueName}

                    configure.Host(rabbitUri, h =>
                    {
                    });


                    configure.ReceiveEndpoint(rabbitSettings.QueueNameConsumer, e =>
                    {
                        e.ConfigureConsumer<PaymentProcessedEventConsumer>(context);

                        var redeliveryIntervals = GetIntervals(rabbitSettings.RedeliveryInSeconds);
                        var retryIntervals = GetIntervals(rabbitSettings.RetryInSeconds);

                        if (!Equals(redeliveryIntervals, null) && redeliveryIntervals.Any())
                        {
                            e.UseScheduledRedelivery(r => r.Intervals(redeliveryIntervals));
                        }

                        if (!Equals(redeliveryIntervals, null) && retryIntervals.Any())
                        {
                            e.UseMessageRetry(r => r.Intervals(retryIntervals));

                        }
                        
                    });
                });
            });
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
