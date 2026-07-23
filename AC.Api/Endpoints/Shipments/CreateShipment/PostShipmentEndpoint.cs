using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Shipments.Commands.CreateShipment;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.CreateShipment;

public class PostShipmentEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateShipmentRequest>
        .WithActionResult<CreateShipmentCommandResult>
{
    [HttpPost("api/v1/core/shipments")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateShipmentCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateShipmentCommandResult>> HandleAsync(
        CreateShipmentRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateShipmentCommand
        {
            OrderDeliveryId = request.OrderDeliveryId,
            PackageCount = request.PackageCount,
            PackageDescription = request.PackageDescription,
            Lines = request.Lines.Select(l => new CreateShipmentLine
            {
                OrderDeliveryDetailId = l.OrderDeliveryDetailId,
                Weight = l.Weight,
                ShippingCost = l.ShippingCost
            }).ToList()
        };

        var result = await mediator.SendCommandAsync<CreateShipmentCommand, CreateShipmentCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "shipment.orderdelivery.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Created($"api/v1/core/shipments/{result.Value.Id}", result.Value);
    }
}

public class CreateShipmentRequest
{
    public Guid OrderDeliveryId { get; set; }
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = null!;
    public List<CreateShipmentLineRequest> Lines { get; set; } = [];
}

public class CreateShipmentLineRequest
{
    public Guid OrderDeliveryDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
