using Comments.API.Interfaces;
using MassTransit;
using Ratbags.Shared.DTOs.Events.Events.CommentsRequest;

namespace Ratbags.Comments.Messaging.Consumers;

public class CommentsConsumer : IConsumer<CommentsForArticleRequest>
{
    private readonly ILogger<CommentsConsumer> _logger;
    private readonly IService _commentsService;

    public CommentsConsumer(
        IService commentsService,
        ILogger<CommentsConsumer> logger)
    {
        _commentsService = commentsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CommentsForArticleRequest> context)
    {
        try
        {
            _logger.LogInformation("listening...");

            var comments = await _commentsService.GetByArticleIdAsync(context.Message.ArticleId);

            _logger.LogInformation($"got {comments.Count()} comments for article {context.Message.ArticleId}");

            // respond to the request
            await context.RespondAsync(new CommentsForArticleResponse
            {
                ArticleId = context.Message.ArticleId,
                Comments = comments.ToList()
            }, ctx =>
            {
                // spare code! pointless but keep for now in case you move away from request/response
                ctx.SetRoutingKey("comments.response");
            });
        }
        catch (Exception e)
        {
            throw;
        }
    }
}