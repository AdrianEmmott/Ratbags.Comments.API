using Ratbags.Comments.API.Models.API;
using Ratbags.Comments.API.Models.DTOs;

namespace Comments.API.Interfaces;

public interface ICommentsService
{
    Task<Guid> CreateAsync(CommentCreate model);
    Task<bool> DeleteAsync(Guid id);
    Task<CommentDTO?> GetByIdAsync(Guid id);
    Task<IEnumerable<CommentDTO>?> GetByArticleIdAsync(Guid id);
    Task<int> GetCountForArticleAsync(Guid id);
    Task<bool> UpdateAsync(CommentUpdate model);
}
