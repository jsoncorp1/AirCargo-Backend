using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.ArticleReceipts.Commands.CreateArticleReceipt;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.ArticleReceipts.CreateArticleReceipt;

public class PostArticleReceiptEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateArticleReceiptRequest>
        .WithActionResult<CreateArticleReceiptCommandResult>
{
    [HttpPost("api/v1/core/article-receipts")]
    [SwaggerOperation(Tags = ["Core / Article Receipts"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateArticleReceiptCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateArticleReceiptCommandResult>> HandleAsync(
        CreateArticleReceiptRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateArticleReceiptCommand
        {
            ArticleId = request.ArticleId,
            Count = request.Count
        };

        var result = await mediator.SendCommandAsync<CreateArticleReceiptCommand, CreateArticleReceiptCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/article-receipts/{result.Value.Id}", result.Value);
    }
}

public class CreateArticleReceiptRequest
{
    public Guid ArticleId { get; set; }
    public int Count { get; set; }
}
