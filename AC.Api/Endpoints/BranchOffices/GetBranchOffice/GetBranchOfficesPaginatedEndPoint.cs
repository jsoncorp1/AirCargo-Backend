using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.BranchOffices.Queries.GetBranchOfficesPaginated;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.BranchOffices.GetBranchOffice;

[Authorize(Roles = RoleNames.SuperAdminAdmin)]
public class GetBranchOfficesPaginatedEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<GetBranchOfficesPaginatedRequest>
        .WithActionResult<GetBranchOfficesPaginatedQueryResult>
{
    [HttpGet("api/v1/core/branch-offices")]
    [SwaggerOperation(Tags = ["Core / BranchOffices"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetBranchOfficesPaginatedQueryResult))]
    public override async Task<ActionResult<GetBranchOfficesPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetBranchOfficesPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetBranchOfficesPaginatedQuery, GetBranchOfficesPaginatedQueryResult>(
            new GetBranchOfficesPaginatedQuery { Page = request.Page, PerPage = request.PerPage },
            cancellationToken);

        return Ok(result.Value);
    }
}

public class GetBranchOfficesPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}
