using Ratbags.Shared.DTOs.Events.DTOs.Articles.Comments;

namespace Comments.API.Interfaces
{
    public interface ICommentsService
    {
        Task<Guid> CreateCommentAsync(CreateCommentDTO commentDTO);
        Task DeleteCommentAsync(Guid id);
        Task<CommentDTO?> GetCommentByIdAsync(Guid id);
        Task<IEnumerable<CommentDTO>> GetCommentsByArticleIdAsync(Guid id);
        Task UpdateCommentAsync(Guid id, CommentDTO comment);
    }
}
