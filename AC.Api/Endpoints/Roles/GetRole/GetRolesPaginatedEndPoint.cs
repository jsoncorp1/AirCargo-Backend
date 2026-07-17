using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Roles.Queries.GetRolesPaginated;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Roles.GetRole;

public class GetRolesPaginatedEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<GetRolesPaginatedRequest>
        .WithActionResult<GetRolesPaginatedQueryResult>
{
    [HttpGet("api/v1/core/roles")]
    [SwaggerOperation(Tags = ["Core / Roles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetRolesPaginatedQueryResult))]
    public override async Task<ActionResult<GetRolesPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetRolesPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetRolesPaginatedQuery, GetRolesPaginatedQueryResult>(
            new GetRolesPaginatedQuery { Page = request.Page, PerPage = request.PerPage },
            cancellationToken);

        return Ok(result.Value);
    }
}

public class GetRolesPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}