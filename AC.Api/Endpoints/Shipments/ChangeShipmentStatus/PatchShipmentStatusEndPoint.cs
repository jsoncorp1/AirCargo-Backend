using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Shipments.Commands.ChangeShipmentStatus;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.ChangeShipmentStatus;

[Authorize(Roles = RoleNames.SuperAdminAdminConductor)]
public class PatchShipmentStatusEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<PatchShipmentStatusRequest>
        .WithActionResult<ChangeShipmentStatusCommandResult>
{
    [HttpPatch("api/v1/core/shipments/{id:guid}/status")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ChangeShipmentStatusCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<ChangeShipmentStatusCommandResult>> HandleAsync(
        PatchShipmentStatusRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ChangeShipmentStatusCommand
        {
            Id = request.Id,
            UserId = currentUser.UserId!.Value,
            Status = request.Body.Status,
            Observation = request.Body.Observation,
            DeliveryComment = request.Body.DeliveryComment
        };

        var result = await mediator.SendCommandAsync<ChangeShipmentStatusCommand, ChangeShipmentStatusCommandResult>(
            command, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}

public class PatchShipmentStatusRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PatchShipmentStatusBody Body { get; set; } = new();
}

public class PatchShipmentStatusBody
{
    // Mandar Status para un cambio manual, u Observation para registrar una
    // observación (el estado se deriva de ella); no ambos a la vez.
    public ShipmentStatus? Status { get; set; }
    public ShipmentObservation? Observation { get; set; }
    public string? DeliveryComment { get; set; }
}
