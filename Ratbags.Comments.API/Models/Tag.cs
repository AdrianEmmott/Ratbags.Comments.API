namespace Ratbags.Comments.API.Models;

public partial class Tag
{
    public Guid TagId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
