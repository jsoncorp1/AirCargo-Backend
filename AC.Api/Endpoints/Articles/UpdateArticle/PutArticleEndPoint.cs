using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Articles.Commands.UpdateArticle;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Articles.UpdateArticle;

public class PutArticleEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<PutArticleRequest>
        .WithActionResult<UpdateArticleCommandResult>
{
    [HttpPut("api/v1/core/articles/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Articles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateArticleCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateArticleCommandResult>> HandleAsync(
        PutArticleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateArticleCommand
        {
            Id = request.Id,
            Sku = request.Body.Sku,
            Name = request.Body.Name,
            Price = request.Body.Price,
            SupplierId = request.Body.SupplierId
        };

        var result = await mediator.SendCommandAsync<UpdateArticleCommand, UpdateArticleCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "article.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}

public class PutArticleRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutArticleBody Body { get; set; } = new();
}

public class PutArticleBody
{
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid SupplierId { get; set; }
}
