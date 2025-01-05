namespace Ratbags.Comments.API.Messaging.Responses
{
    public class CommentsForArticleResponse
    {
        public Guid ArticleId { get; set; }
        public string Comments { get; set; } = default!; // will hold JSON
    }
}
