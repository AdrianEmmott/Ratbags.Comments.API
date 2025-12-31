using Ratbags.Comments.API.Models;
using Ratbags.Core.Messaging.ASB.RequestReponse;

namespace Ratbags.Comments.API.Messaging;

public sealed class GetCommentsForArticleWorker 
    : ServiceBusRequestReplyWorker<GetCommentsForArticleRequest, GetCommentsForArticleResponse>
{
    public GetCommentsForArticleWorker(
        AppSettings appSettings,
        IServiceScopeFactory scopeFactory,
        ILogger<GetCommentsForArticleWorker> logger)
        : base(
            appSettings.Messaging.ASB.Connection,
            appSettings.MessagingExtensions.CommentsListTopic,
            appSettings.Messaging.ASB.ResponseSubscription,
            scopeFactory,
            logger)
    { }
}