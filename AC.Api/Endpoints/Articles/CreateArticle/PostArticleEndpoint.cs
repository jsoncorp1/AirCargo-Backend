using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Articles.Commands.CreateArticle;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Articles.CreateArticle;

public class PostArticleEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateArticleRequest>
        .WithActionResult<CreateArticleCommandResult>
{
    [HttpPost("api/v1/core/articles")]
    [SwaggerOperation(Tags = ["Core / Articles"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateArticleCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateArticleCommandResult>> HandleAsync(
        CreateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateArticleCommand
        {
            Sku = request.Sku,
            Name = request.Name,
            Price = request.Price,
            SupplierId = request.SupplierId
        };

        var result = await mediator.SendCommandAsync<CreateArticleCommand, CreateArticleCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/articles/{result.Value.Id}", result.Value);
    }
}

public class CreateArticleRequest
{
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid SupplierId { get; set; }
}
