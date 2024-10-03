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
                        c.SetEntityName("articles.comments.exchange"); // Set exchange name for this message type
                    });

                    cfg.ReceiveEndpoint("articles.comments.queue", q =>
                    {
                        q.ConfigureConsumer<CommentsConsumer>(context);

                        // Bind queue to the specific exchange
                        q.Bind("articles.comments.exchange", e =>
                        {
                            e.RoutingKey = "request"; // Match routing key with articles API
                        });

                        // Disable the auto-creation of the default queue exchange
                        q.ConfigureConsumeTopology = false;
                    });
                });
            });

            return services;
        }
    }
}
