using Ratbags.Shared.DTOs.Events.DTOs.Articles.Comments;

namespace Comments.API.Interfaces
{
    public interface IService
    {
        Task<Guid> CreateAsync(CreateCommentDTO commentDTO);
        Task<bool> DeleteAsync(Guid id);
        Task<CommentDTO?> GetByIdAsync(Guid id);
        Task<IEnumerable<CommentDTO>> GetByArticleIdAsync(Guid id);
        Task<bool> UpdateAsync(CommentDTO comment);
    }
}
