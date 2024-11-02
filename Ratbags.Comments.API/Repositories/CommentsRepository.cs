using Microsoft.EntityFrameworkCore;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models.DB;

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

    public async Task<Guid> CreateAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        return comment.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var model = new Comment { Id = id };
        _context.Comments.Attach(model);
        _context.Comments.Remove(model);
        await _context.SaveChangesAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public IQueryable<Comment> GetQueryable()
    {
        return _context.Comments;
    }

    public async Task<List<Comment>> GetByArticleId(Guid articleId)
    {
        var results = _context.Comments
            .OrderBy(x => x.PublishDate)
            .Where(x => x.ArticleId == articleId);

        return await results.ToListAsync();
    }

    public async Task<int> GetCountByArticle(Guid id)
    {
        var commentsCount = await _context
            .Comments
            .Where(x => x.ArticleId == id)
            .CountAsync();

        return commentsCount;
    }

    public async Task UpdateAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }
}