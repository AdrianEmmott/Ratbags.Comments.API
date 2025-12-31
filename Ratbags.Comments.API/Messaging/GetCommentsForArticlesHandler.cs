using Comments.API.Interfaces;
using Ratbags.Core.Messaging.ASB.RequestReponse;

namespace Ratbags.Comments.API.Messaging;

public sealed class GetCommentsForArticlesHandler
    : IServiceBusRequestHandler<GetCommentsForArticleRequest, GetCommentsForArticleResponse>
{
    private readonly ICommentsService _service;

    public GetCommentsForArticlesHandler(ICommentsService service)
    {
        _service = service;
    }

    public async Task<GetCommentsForArticleResponse> HandleAsync
        (GetCommentsForArticleRequest request, CancellationToken ct)
    {
        var comments = await _service.GetByArticleIdAsync(request.articleId);

        return new GetCommentsForArticleResponse(comments: comments);
    }
}
