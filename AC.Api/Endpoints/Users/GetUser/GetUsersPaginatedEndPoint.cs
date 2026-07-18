using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Users.Queries.GetUsersPaginated;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Users.GetUser;

public class GetUsersPaginatedEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<GetUsersPaginatedRequest>
        .WithActionResult<GetUsersPaginatedQueryResult>
{
    [HttpGet("api/v1/core/users")]
    [SwaggerOperation(Tags = ["Core / Users"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetUsersPaginatedQueryResult))]
    public override async Task<ActionResult<GetUsersPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetUsersPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetUsersPaginatedQuery, GetUsersPaginatedQueryResult>(
            new GetUsersPaginatedQuery { Page = request.Page, PerPage = request.PerPage },
            cancellationToken);

        return Ok(result.Value);
    }
}

public class GetUsersPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}