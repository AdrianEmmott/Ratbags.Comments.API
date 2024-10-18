using Ratbags.Core.DTOs.Articles;
using Ratbags.Core.Models.Articles;

namespace Comments.API.Interfaces;

public interface IService
{
    Task<Guid> CreateAsync(CreateCommentModel model);
    Task<bool> DeleteAsync(Guid id);
    Task<CommentDTO?> GetByIdAsync(Guid id);
    Task<IEnumerable<CommentDTO>> GetByArticleIdAsync(Guid id);
    Task<int> GetCommentsCountForArticleAsync(Guid id);
    Task<bool> UpdateAsync(UpdateCommentModel model);
}
