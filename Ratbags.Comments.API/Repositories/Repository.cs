using Microsoft.EntityFrameworkCore;
using Ratbags.Comments.API.Interfaces;
using Ratbags.Comments.API.Models.DB;

namespace Ratbags.Comments.API.Repositories;

public class Repository : IRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<Repository> _logger;

    public Repository(ApplicationDbContext context,
        ILogger<Repository> logger)
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

    public async Task<IEnumerable<Comment?>> GetByArticleIdAsync(Guid id)
    {
        return await _context.Comments
                                .Where(x => x.ArticleId == id)
                                .OrderByDescending(x => x.PublishDate)
                                .ToListAsync();
    }

    public async Task UpdateAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }
}