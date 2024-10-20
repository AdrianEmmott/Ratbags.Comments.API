using MassTransit;
using Ratbags.Comments.Messaging.Consumers;
using Ratbags.Core.Events.Accounts;
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
            x.AddConsumer<CommentsCountConsumer>();

            // rabbitmq config
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{appSettings.Messaging.Hostname}/{appSettings.Messaging.VirtualHost}", h =>
                {
                    h.Username(appSettings.Messaging.Username);
                    h.Password(appSettings.Messaging.Password);
                });

                Console.WriteLine($"*** appSettings.Messaging.Hostname: {appSettings.Messaging.Hostname}");
                Console.WriteLine($"*** appSettings.Messaging.VirtualHost: {appSettings.Messaging.VirtualHost}");
                Console.WriteLine($"*** appSettings.Messaging.Username: {appSettings.Messaging.Username}");
                Console.WriteLine($"*** appSettings.Messaging.Password: {appSettings.Messaging.Password}");

                cfg.Message<UserFullNameRequest>(c =>
                {
                    c.SetEntityName("accounts.user-full-name"); // set exchange name for this message type
                });

                cfg.Message<CommentsForArticleResponse>(c =>
                {
                    c.SetEntityName("articles.comments"); // exchange name for message type
                });

                cfg.Message<CommentsCountForArticleResponse>(c =>
                {
                    c.SetEntityName("articles.comments-count"); // exchange name for message type
                });

                cfg.ReceiveEndpoint("articles.comments", q =>
                {
                    q.ConfigureConsumer<CommentsConsumer>(context);
                });

                cfg.ReceiveEndpoint("articles.comments-count", q =>
                {
                    q.ConfigureConsumer<CommentsCountConsumer>(context);
                });
            });
        });

        return services;
    }
}