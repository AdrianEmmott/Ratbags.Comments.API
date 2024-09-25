using Microsoft.EntityFrameworkCore;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models;

namespace Ratbags.Comments.API.Repositories;

public class CommentsRepository : ICommentsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CommentsRepository> _logger;

    public CommentsRepository(ApplicationDbContext context, 
        ILogger<CommentsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> CreateCommentAsync(Comment comment)
    {
        try
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return comment.Id;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error creating comment for article {comment.ArticleId}: {e.Message}");
            throw;
        }
    }

    public async Task DeleteCommentAsync(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid id)
    {
        _logger.LogInformation($"get comment {id}");
        return await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Comment?>> GetCommentsByArticleIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation($"get comments for article {id}");
            return await _context.Comments
                                    .Where(x => x.ArticleId == id)
                                    .OrderByDescending(x => x.PublishDate)
                                    .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogInformation($"error - get comments for article {id}: {e.Message}");
            throw;
        }
    }

    public async Task UpdateCommentAsync(Guid id, Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }
}