using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptsPaginated;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.ArticleReceipts.GetArticleReceipt;

public class GetArticleReceiptsPaginatedEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<GetArticleReceiptsPaginatedRequest>
        .WithActionResult<GetArticleReceiptsPaginatedQueryResult>
{
    [HttpGet("api/v1/core/article-receipts")]
    [SwaggerOperation(Tags = ["Core / Article Receipts"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticleReceiptsPaginatedQueryResult))]
    public override async Task<ActionResult<GetArticleReceiptsPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetArticleReceiptsPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticleReceiptsPaginatedQuery, GetArticleReceiptsPaginatedQueryResult>(
            new GetArticleReceiptsPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                ArticleId = request.ArticleId
            },
            cancellationToken);

        return Ok(result.Value);
    }
}

public class GetArticleReceiptsPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? ArticleId { get; set; }
}
