using System.ComponentModel.DataAnnotations;

namespace Ratbags.Comments.API.Models.API;

public class CommentCreate
{
    [Required(ErrorMessage = "Article id is required")]
    public Guid ArticleId { get; set; }

    [Required(ErrorMessage = "Comment is required")]
    public string Content { get; set; } = default!;

    [Required(ErrorMessage = "User id is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Publish date is required")]
    public DateTime Published { get; set; }
}
