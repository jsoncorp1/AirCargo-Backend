using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Roles.Queries.GetRoleById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Roles.GetRole;

public class GetRoleByIdEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetRoleByIdQueryResult>
{
    [HttpGet("api/v1/core/roles/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Roles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetRoleByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetRoleByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetRoleByIdQuery, GetRoleByIdQueryResult>(
            new GetRoleByIdQuery { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}