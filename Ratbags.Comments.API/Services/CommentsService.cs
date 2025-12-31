using Comments.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models;
using Ratbags.Comments.API.Models.API;
using Ratbags.Comments.API.Models.DB;
using Ratbags.Comments.API.Models.DTOs;
using Ratbags.Core.DTOs.Articles;

namespace Ratbags.Comments.API.Services;

public class CommentsService : ICommentsService
{
    private readonly AppSettings _appSettings;
    private readonly ICommentsRepository _repository;
    private readonly ILogger<CommentsService> _logger;

    public CommentsService(
        AppSettings appSettings,
        ICommentsRepository repository,
        ILogger<CommentsService> logger)
    {
        _appSettings = appSettings;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> CreateAsync(CommentCreate model)
    {
        if (model.ArticleId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(model.ArticleId));
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = model.ArticleId,
            CommentContent = model.Content,
            PublishDate = model.Published == DateTime.MinValue ? model.Published : DateTime.UtcNow,
            UserId = model.UserId
        };

        try
        {
            var commentId = await _repository.CreateAsync(comment);

            return commentId;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError($"Error inserting comment for article {model.ArticleId}: {e.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var comment = await _repository.GetByIdAsync(id);

            if (comment == null)
            {
                return false;
            }

            await _repository.DeleteAsync(id);
            return true;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError($"Error deleting comment {id}: {e.Message}");
            throw;
        }
    }

    public async Task<CommentCoreDTO?> GetByIdAsync(Guid id)
    {
        var comment = await _repository.GetByIdAsync(id);

        if (comment != null)
        {
            return new CommentCoreDTO
            (
                Id: comment.Id,
                UserId: comment.UserId,
                Published: comment.PublishDate,
                Content: comment.CommentContent
            );
        }

        return null;
    }
      
    public async Task<IEnumerable<CommentCoreDTO>?> GetByArticleIdAsync(Guid id)
    {
        var comments = await _repository.GetByArticleId(id);

        if (comments != null && comments.Count() > 0)
        {
            var commentDTOs = comments.Select(comment => new CommentCoreDTO
            (
                Id: comment.Id,
                UserId: comment.UserId,
                Published: comment.PublishDate,
                Content: comment.CommentContent
            ))
            .OrderBy(x => x.Published)
            .ToList();

            _logger.LogInformation($"Found {comments.Count()} comments for article {id}");

            return commentDTOs;
        }

        return null;
    }

    public async Task<int> GetCountForArticleAsync(Guid id)
    {
        var commentsCount = await _repository.GetCountByArticle(id);

        _logger.LogInformation($"Counted {commentsCount } comments for article {id}");

        return commentsCount;
    }

    public async Task<Dictionary<Guid, int>> GetCountsForArticlesAsync(IReadOnlyList<Guid> ids)
    {
        var commentsCount = await _repository.GetCountByArticlesAsync(ids);

        _logger.LogInformation($"Retrieved comment counts for {ids.Count()} articles");

        return commentsCount;
    }

    public async Task<bool> UpdateAsync(CommentUpdate model)
    {
        var existingComment = await _repository.GetByIdAsync(model.Id);

        if (existingComment == null)
        {
            return false;
        }

        try
        {
            existingComment.CommentContent = model.Content;
            existingComment.PublishDate = DateTime.UtcNow;

            await _repository.UpdateAsync(existingComment);

            return true;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError($"Error updating comment {model.Id}: {e.Message}");
            throw;
        }
    }
}