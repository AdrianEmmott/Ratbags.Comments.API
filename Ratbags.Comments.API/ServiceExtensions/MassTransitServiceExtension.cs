using MassTransit;
using Ratbags.Comments.Messaging.Consumers;
using Ratbags.Core.Events.CommentsRequest;
using Ratbags.Core.Settings;

namespace Ratbags.Comments.API.ServiceExtensions;

public static class MassTransitServiceExtension
{
    public static IServiceCollection AddMassTransitWithRabbitMqServiceExtension(this IServiceCollection services, AppSettingsBase appSettings)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<CommentsConsumer>();

            // rabbitmq config
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{appSettings.Messaging.Hostname}/{appSettings.Messaging.VirtualHost}", h =>
                {
                    h.Username(appSettings.Messaging.Username);
                    h.Password(appSettings.Messaging.Password);
                });

                cfg.Message<CommentsForArticleResponse>(c =>
                {
                    c.SetEntityName("articles.comments"); // exchange name for message type
                });

                cfg.ReceiveEndpoint("articles.comments", q =>
                {
                    q.ConfigureConsumer<CommentsConsumer>(context);

                    // bind queue to the specific exchange
                    q.Bind("articles.comments", e =>
                    {
                        e.RoutingKey = "request"; // match articles api - pretty sure this isn't needed
                    });
                });
            });
        });

        return services;
    }
}