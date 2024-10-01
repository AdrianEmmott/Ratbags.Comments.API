using Ratbags.Comments.API.Models;
using Ratbags.Shared.DTOs.Events.DTOs.Articles.Comments;

namespace Ratbags.Comments.API.Interfaces
{
    public interface IRepository
    {
        Task<Guid> CreateAsync(Comment comment);
        Task DeleteAsync(Guid id);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Comment?>> GetByArticleIdAsync(Guid id);
        Task UpdateAsync(Comment comment);
    }
}
