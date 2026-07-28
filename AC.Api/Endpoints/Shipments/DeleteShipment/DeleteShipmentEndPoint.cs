using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Shipments.Commands.DeleteShipment;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.DeleteShipment;

[Authorize(Roles = RoleNames.SuperAdminAdmin)]
public class DeleteShipmentEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteShipmentCommandResult>
{
    [HttpDelete("api/v1/core/shipments/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteShipmentCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteShipmentCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteShipmentCommand, DeleteShipmentCommandResult>(
            new DeleteShipmentCommand { Id = id, UserId = currentUser.UserId!.Value }, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}
