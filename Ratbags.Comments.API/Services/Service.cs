using Comments.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models.DB;
using Ratbags.Core.DTOs.Articles;
using Ratbags.Core.Models.Articles;

namespace Ratbags.Comments.API.Services;

public class Service : IService
{
    private readonly IRepository _repository;
    private readonly ILogger<Service> _logger;

    public Service(IRepository repository,
        ILogger<Service> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> CreateAsync(CreateCommentModel model)
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

    public async Task<CommentDTO?> GetByIdAsync(Guid id)
    {
        var comment = await _repository.GetByIdAsync(id);

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
      
    public async Task<IEnumerable<CommentDTO>> GetByArticleIdAsync(Guid id)
    {
        _logger.LogInformation($"get comments for article {id}");

        var comments = _repository.GetQueryable()
            .OrderBy(c => c.PublishDate)
            .Where(c => c.ArticleId == id);

        if (comments != null && comments.Count() > 0)
        {
            return comments.Select(comment => new CommentDTO
            {
                Id = comment.Id,
                ArticleId = comment.ArticleId,
                Published = comment.PublishDate,
                Content = comment.CommentContent
            })
            .OrderBy(x => x.Published)
            .ToList();
        }

        return new List<CommentDTO>();
    }

    public async Task<int> GetCommentsCountForArticleAsync(Guid id)
    {
        _logger.LogInformation($"get comments count for article {id}");

        var commentsCount = await _repository.GetCommentsCountByArticle(id);

        return commentsCount;
    }

    public async Task<bool> UpdateAsync(UpdateCommentModel model)
    {
        var existingComment = await _repository.GetByIdAsync(model.Id);

        if (existingComment == null)
        {
            return false;
        }

        try
        {
            existingComment.CommentContent = model.Content;

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