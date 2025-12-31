using Azure;
using Ratbags.Comments.API.Models;
using Ratbags.Core.Messaging.ASB.RequestReponse;

namespace Ratbags.Comments.API.Messaging;

public sealed class GetCommentCountsForArticlesWorker 
    : ServiceBusRequestReplyWorker<GetCommentCountsForArticlesRequest, GetCommentCountsForArticlesResponse>
{
    public GetCommentCountsForArticlesWorker(
        AppSettings appSettings,
        IServiceScopeFactory scopeFactory,
        ILogger<GetCommentCountsForArticlesWorker> logger)
        : base(
            appSettings.Messaging.ASB.Connection,
            appSettings.MessagingExtensions.CommentsCountTopic,
            appSettings.Messaging.ASB.ResponseSubscription,
            scopeFactory,
            logger)
    { }
}