namespace Ratbags.Comments.API.Models.DB;

public partial class Comment
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }

    public Guid UserId { get; set; }

    public string CommentContent { get; set; } = default!;

    public DateTime PublishDate { get; set; }
}
