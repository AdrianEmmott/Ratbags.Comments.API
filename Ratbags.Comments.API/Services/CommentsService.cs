using Comments.API.Interfaces;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models;
using Ratbags.Shared.DTOs.Events.DTOs.Articles.Comments;

namespace Ratbags.Comments.API.Services;

public class CommentsService : ICommentsService
{
    private readonly ICommentsRepository _repository;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly ILogger<CommentsService> _logger;

    public CommentsService(ICommentsRepository repository, 
        IServiceScopeFactory serviceProvider,
        ILogger<CommentsService> logger)
    {
        _repository = repository;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<Guid> CreateCommentAsync(CreateCommentDTO commentDTO)
    {
        if (commentDTO.ArticleId == Guid.Empty) {
            throw new ArgumentNullException(nameof(commentDTO.ArticleId));
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = commentDTO.ArticleId,
            CommentContent = commentDTO.Content,
            PublishDate = commentDTO.Published == DateTime.MinValue ? commentDTO.Published : DateTime.UtcNow,
            UserId = commentDTO.UserId
        };

        var commentId = await _repository.CreateCommentAsync(comment);

        return commentId;
    }

    public async Task DeleteCommentAsync(Guid id)
    {
        await _repository.DeleteCommentAsync(id);
    }

    public async Task<CommentDTO?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _repository.GetCommentByIdAsync(id);

        if (comment != null)
        {
            return new CommentDTO
            {
                Id = comment.Id,
                ArticleId = comment.ArticleId,
                Published = comment.PublishDate,
                Content = comment.CommentContent
            };
        }

        return null;
    }

    public async Task<IEnumerable<CommentDTO>> GetCommentsByArticleIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation($"get comments for article {id}");
            var comments = await _repository.GetCommentsByArticleIdAsync(id);

            if (comments != null)
            {
                return comments.Select(comment => new CommentDTO
                {
                    Id = comment.Id,
                    ArticleId = comment.ArticleId,
                    Published = comment.PublishDate,
                    Content = comment.CommentContent
                });
            }

            return null;
        }
        catch (Exception e)
        {
            _logger.LogInformation($"error - get comments for article {id}: {e.Message}");
            throw;
        }
    }

    public async Task UpdateCommentAsync(Guid id, CommentDTO commentDTO)
    {
        var existingComment = await _repository.GetCommentByIdAsync(id);

        if (existingComment == null)
        {
            throw new Exception("Comment not found");
        }

        existingComment.CommentContent = commentDTO.Content;

        await _repository.UpdateCommentAsync(id, existingComment);
    }
}