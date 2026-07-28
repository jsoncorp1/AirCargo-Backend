using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Users.Queries.GetUsersPaginated;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Users.GetUser;

[Authorize(Roles = RoleNames.SuperAdminAdmin)]
public class GetUsersPaginatedEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<GetUsersPaginatedRequest>
        .WithActionResult<GetUsersPaginatedQueryResult>
{
    [HttpGet("api/v1/core/users")]
    [SwaggerOperation(Tags = ["Core / Users"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetUsersPaginatedQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetUsersPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetUsersPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetUsersPaginatedQuery, GetUsersPaginatedQueryResult>(
            new GetUsersPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                UserId = currentUser.UserId!.Value,
                RoleId = request.Role,
                SupplierId = request.SupplierId
            },
            cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}

public class GetUsersPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? Role { get; set; }
    public Guid? SupplierId { get; set; }
}