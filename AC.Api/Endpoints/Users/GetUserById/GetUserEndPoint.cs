using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Users.Queries.GetUserById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Users.GetUserById;

public class GetUserEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetUserByIdQueryResult>
{
    [HttpGet("api/v1/core/users/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Users"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetUserByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetUserByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetUserByIdQuery, GetUserByIdQueryResult>(
            new GetUserByIdQuery { Id = id }, cancellationToken);

        if (result.Failure)
            return result.ErrorKey.EndsWith(".notfound")
                ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
                : BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error });

        return Ok(result.Value);
    }
}