namespace Ratbags.Comments.API.Models;

public partial class Author
{
    public Guid AuthorId { get; set; }

    public Guid UserId { get; set; }

    public virtual AspNetUser User { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
