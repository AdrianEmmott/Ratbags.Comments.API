using Comments.API.Interfaces;
using Ratbags.Core.Messaging.ASB.RequestReponse;

namespace Ratbags.Comments.API.Messaging;

public sealed class GetCommentCountsForArticlesHandler
    : IServiceBusRequestHandler<GetCommentCountsForArticlesRequest, GetCommentCountsForArticlesResponse>
{
    private readonly ICommentsService _service;

    public GetCommentCountsForArticlesHandler(ICommentsService service)
    {
        _service = service;
    }

    public async Task<GetCommentCountsForArticlesResponse> HandleAsync(
        GetCommentCountsForArticlesRequest request, 
        CancellationToken ct)
    {
        var results = await _service.GetCountsForArticlesAsync(request.ArticleIds);

        return new GetCommentCountsForArticlesResponse(Counts: results);
    }    
}
