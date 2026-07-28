using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Shipments.Queries.GetShipmentById;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.GetShipment;

[Authorize(Roles = RoleNames.Todos)]
public class GetShipmentByIdEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetShipmentByIdQueryResult>
{
    [HttpGet("api/v1/core/shipments/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetShipmentByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetShipmentByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetShipmentByIdQuery, GetShipmentByIdQueryResult>(
            new GetShipmentByIdQuery { Id = id, UserId = currentUser.UserId!.Value }, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}
