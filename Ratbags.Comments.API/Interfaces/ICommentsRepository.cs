using Ratbags.Comments.API.Models.DB;

namespace Ratbags.Comments.API.Interfaces;

public interface ICommentsRepository
{
    Task<Guid> CreateAsync(Comment comment);
    Task DeleteAsync(Guid id);
    Task<Comment?> GetByIdAsync(Guid id);

    Task<int> GetCommentsCountByArticle(Guid id);

    IQueryable<Comment> GetQueryable(); // TODO hmm
    Task UpdateAsync(Comment comment);
}
