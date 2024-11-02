namespace Ratbags.Comments.API.Models.DTOs
{
    public class CommentDTO
    {
        public Guid Id { get; set; }

        public string CommenterName { get; set; } = string.Empty;

        public string Content { get; set; } = default!;

        public DateTime Published { get; set; }
    }
}
