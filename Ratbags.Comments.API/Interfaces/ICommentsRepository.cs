using Ratbags.Comments.API.Models;
using Ratbags.Shared.DTOs.Events.DTOs.Articles.Comments;

namespace Ratbags.Comments.API.Interfaces
{
    public interface ICommentsRepository
    {
        Task<Guid> CreateCommentAsync(Comment comment);
        Task DeleteCommentAsync(Guid id);
        Task<Comment?> GetCommentByIdAsync(Guid id);
        Task<IEnumerable<Comment?>> GetCommentsByArticleIdAsync(Guid id);
        Task UpdateCommentAsync(Guid id, Comment comment);
    }
}
