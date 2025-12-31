using Ratbags.Core.DTOs.Articles;

namespace Ratbags.Comments.API.Messaging;

// article comments
public sealed record GetCommentsForArticleRequest(Guid articleId);
public sealed record GetCommentsForArticleResponse(IEnumerable<CommentCoreDTO>? comments);

// article comment counts
public sealed record GetCommentCountsForArticlesRequest(IReadOnlyList<Guid> ArticleIds);
public sealed record GetCommentCountsForArticlesResponse(Dictionary<Guid, int> Counts);
