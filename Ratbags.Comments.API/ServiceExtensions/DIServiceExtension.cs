using Comments.API.Interfaces;
using Microsoft.Extensions.Options;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Messaging;
using Ratbags.Comments.API.Models;
using Ratbags.Comments.API.Repositories;
using Ratbags.Comments.API.Services;
using Ratbags.Core.Messaging.ASB.RequestReponse;

namespace Ratbags.Comments.API.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ICommentsRepository, CommentsRepository>();
        services.AddScoped<ICommentsService, CommentsService>();

        // expose appSettings as IOptions<T> singleton
        services.AddSingleton(x => x.GetRequiredService<IOptions<AppSettings>>().Value);

        // service bus
        services.AddHostedService<GetCommentCountsForArticlesWorker>();
        services.AddHostedService<GetCommentsForArticleWorker>();

        services.AddScoped<IServiceBusRequestHandler
            <GetCommentsForArticleRequest, GetCommentsForArticleResponse>, GetCommentsForArticlesHandler>();

        services.AddScoped<IServiceBusRequestHandler
            <GetCommentCountsForArticlesRequest, GetCommentCountsForArticlesResponse>, GetCommentCountsForArticlesHandler>();

        return services;
    }
}
