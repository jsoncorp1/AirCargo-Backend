using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Shipments.Commands.UpdateShipment;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.UpdateShipment;

public class PutShipmentEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<PutShipmentRequest>
        .WithActionResult<UpdateShipmentCommandResult>
{
    [HttpPut("api/v1/core/shipments/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateShipmentCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateShipmentCommandResult>> HandleAsync(
        PutShipmentRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateShipmentCommand
        {
            Id = request.Id,
            PackageCount = request.Body.PackageCount,
            PackageDescription = request.Body.PackageDescription,
            Lines = request.Body.Lines.Select(l => new UpdateShipmentLine
            {
                ShipmentDetailId = l.ShipmentDetailId,
                Weight = l.Weight,
                ShippingCost = l.ShippingCost
            }).ToList()
        };

        var result = await mediator.SendCommandAsync<UpdateShipmentCommand, UpdateShipmentCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "shipment.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}

public class PutShipmentRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutShipmentBody Body { get; set; } = new();
}

public class PutShipmentBody
{
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = null!;
    public List<PutShipmentLineBody> Lines { get; set; } = [];
}

public class PutShipmentLineBody
{
    public Guid ShipmentDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
