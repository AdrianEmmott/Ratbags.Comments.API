using Ratbags.Shared.DTOs.Events.AppSettingsBase;
using MassTransit;
using Ratbags.Comments.Messaging.Consumers;
using Ratbags.Shared.DTOs.Events.Events.CommentsRequest;

namespace Ratbags.Comments.API.ServiceExtensions
{
    public static class MassTransitServiceExtension
    {
        public static IServiceCollection AddMassTransitWithRabbitMqServiceExtension(this IServiceCollection services, AppSettingsBase appSettings)
        {
            services.AddMassTransit(x =>
            {
                // Register consumers
                x.AddConsumer<CommentsConsumer>();

                // RabbitMQ configuration
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host($"rabbitmq://{appSettings.Messaging.Hostname}/{appSettings.Messaging.VirtualHost}", h =>
                    {
                        h.Username(appSettings.Messaging.Username);
                        h.Password(appSettings.Messaging.Password);
                    });

                    cfg.Message<CommentsForArticleResponse>(c =>
                    {
                        c.SetEntityName("articles.comments"); // Set exchange name for this message type
                    });

                    cfg.ReceiveEndpoint("articles.comments", q =>
                    {
                        q.ConfigureConsumer<CommentsConsumer>(context);

                        // Bind queue to the specific exchange
                        q.Bind("articles.comments", e =>
                        {
                            e.RoutingKey = "request"; // Match routing key with articles API
                        });
                    });
                });
            });

            return services;
        }
    }
}