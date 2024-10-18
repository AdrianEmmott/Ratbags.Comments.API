using Comments.API.Interfaces;
using MassTransit;
using Ratbags.Core.Events.CommentsRequest;

namespace Ratbags.Comments.Messaging.Consumers;

public class CommentsCountConsumer : IConsumer<CommentsCountForArticleRequest>
{
    private readonly ILogger<CommentsCountConsumer> _logger;
    private readonly IService _commentsService;

    public CommentsCountConsumer(
        IService commentsService,
        ILogger<CommentsCountConsumer> logger)
    {
        _commentsService = commentsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CommentsCountForArticleRequest> context)
    {
        _logger.LogInformation("listening...");

        var count = await _commentsService.GetCommentsCountForArticleAsync(context.Message.ArticleId);

        _logger.LogInformation($"{count} comments for article {context.Message.ArticleId}");

        // respond to the request
        await context.RespondAsync(new CommentsCountForArticleResponse
        {
            ArticleId = context.Message.ArticleId,
            Count = count
        }, ctx =>
        {
            // spare code! pointless but keep for now in case you move away from request/response
            ctx.SetRoutingKey("comments-count.response");
        });
    }
}