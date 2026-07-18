using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Articles.Queries.GetArticleById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Articles.GetArticle;

public class GetArticleByIdEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetArticleByIdQueryResult>
{
    [HttpGet("api/v1/core/articles/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Articles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticleByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetArticleByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticleByIdQuery, GetArticleByIdQueryResult>(
            new GetArticleByIdQuery { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
