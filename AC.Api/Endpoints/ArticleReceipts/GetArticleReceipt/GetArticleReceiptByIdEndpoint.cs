using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.ArticleReceipts.GetArticleReceipt;

public class GetArticleReceiptByIdEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetArticleReceiptByIdQueryResult>
{
    [HttpGet("api/v1/core/article-receipts/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Article Receipts"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticleReceiptByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetArticleReceiptByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticleReceiptByIdQuery, GetArticleReceiptByIdQueryResult>(
            new GetArticleReceiptByIdQuery { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
