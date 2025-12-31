using Ratbags.Comments.API.Models.API;
using Ratbags.Comments.API.Models.DTOs;
using Ratbags.Core.DTOs.Articles;

namespace Comments.API.Interfaces;

public interface ICommentsService
{
    Task<Guid> CreateAsync(CommentCreate model);
    Task<bool> DeleteAsync(Guid id);
    Task<CommentCoreDTO?> GetByIdAsync(Guid id);
    Task<IEnumerable<CommentCoreDTO>?> GetByArticleIdAsync(Guid id);
    Task<int> GetCountForArticleAsync(Guid id);
    Task<Dictionary<Guid, int>> GetCountsForArticlesAsync(IReadOnlyList<Guid> ids);
    Task<bool> UpdateAsync(CommentUpdate model);
}
