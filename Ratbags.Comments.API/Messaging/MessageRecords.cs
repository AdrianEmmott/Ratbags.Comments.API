namespace Ratbags.Comments.API.Messaging;

// article comment counts
public sealed record GetCommentCountsForArticlesRequest(IReadOnlyList<Guid> ArticleIds);
public sealed record GetCommentCountsForArticlesResponse(Dictionary<Guid, int> Counts);
