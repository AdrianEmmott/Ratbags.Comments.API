using Comments.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models.DB;
using Ratbags.Core.DTOs.Articles;
using Ratbags.Core.Models.Articles;

namespace Ratbags.Comments.API.Services;

public class CommentsService : ICommentsService
{
    private readonly ICommentsRepository _repository;
    private readonly IMassTransitService _massTransitService;
    private readonly ILogger<CommentsService> _logger;

    public CommentsService(
        ICommentsRepository repository,
        IMassTransitService massTransitService,
        ILogger<CommentsService> logger)
    {
        _repository = repository;
        _massTransitService = massTransitService;
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
            var commentDTOs = comments.Select(comment => new CommentDTO
            {
                Id = comment.Id,
                Published = comment.PublishDate,
                Content = comment.CommentContent
            })
            .OrderBy(x => x.Published)
            .ToList();

            foreach (var item in commentDTOs)
            {
                // TODO NO NO NO NO NO
                var commenterUserId = comments.Where(x => x.Id == item.Id).Select(x => x.UserId).FirstOrDefault();
                var commenterName = await _massTransitService.GetUserNameDetailsAsync(commenterUserId);
                item.CommenterName = commenterName;
            }

            return commentDTOs;
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