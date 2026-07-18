using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.ArticleReceipts.Commands.UpdateArticleReceipt;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.ArticleReceipts.UpdateArticleReceipt;

public class PutArticleReceiptEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<PutArticleReceiptRequest>
        .WithActionResult<UpdateArticleReceiptCommandResult>
{
    [HttpPut("api/v1/core/article-receipts/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Article Receipts"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateArticleReceiptCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateArticleReceiptCommandResult>> HandleAsync(
        PutArticleReceiptRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateArticleReceiptCommand
        {
            Id = request.Id,
            Count = request.Body.Count
        };

        var result = await mediator.SendCommandAsync<UpdateArticleReceiptCommand, UpdateArticleReceiptCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "articlereceipt.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}

public class PutArticleReceiptRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutArticleReceiptBody Body { get; set; } = new();
}

public class PutArticleReceiptBody
{
    public int Count { get; set; }
}
