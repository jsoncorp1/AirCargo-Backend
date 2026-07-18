using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.ArticleReceipts.Commands.DeleteArticleReceipt;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.ArticleReceipts.DeleteArticleReceipt;

public class DeleteArticleReceiptEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteArticleReceiptCommandResult>
{
    [HttpDelete("api/v1/core/article-receipts/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Article Receipts"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteArticleReceiptCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteArticleReceiptCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteArticleReceiptCommand, DeleteArticleReceiptCommandResult>(
            new DeleteArticleReceiptCommand { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
