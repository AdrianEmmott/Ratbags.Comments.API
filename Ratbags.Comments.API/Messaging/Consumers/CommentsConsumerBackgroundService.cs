using Comments.API.Interfaces;
using MassTransit;
using Ratbags.Shared.DTOs.Events.Events.CommentsRequest;

namespace Ratbags.Comments.Messaging.Consumers;

public class CommentsConsumer : IConsumer<CommentsForArticleRequest>
{
    private readonly ILogger<CommentsConsumer> _logger;
    private readonly IService _commentsService;

    private readonly string _exchangeName = "articles.comments.exchange";
    private readonly string _requestQueueName = "comments.request";
    private readonly string _requestRoutingKey = "request";
    private readonly string _responseQueueName = "comments.response";
    private readonly string _responseRoutingKey = "response";

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

            // Respond to the request
            await context.RespondAsync(new CommentsForArticleResponse
            {
                ArticleId = context.Message.ArticleId,
                Comments = comments.ToList()
            });
        }
        catch (Exception e)
        {
            throw;
        }
    }
}