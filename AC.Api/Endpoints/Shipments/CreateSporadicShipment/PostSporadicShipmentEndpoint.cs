using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Shipments.Commands.CreateSporadicShipment;
using AC.Domain.Modules.OrderDeliveries;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.CreateSporadicShipment;

public class PostSporadicShipmentEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<CreateSporadicShipmentRequest>
        .WithActionResult<CreateSporadicShipmentCommandResult>
{
    [HttpPost("api/v1/core/shipments/sporadic")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateSporadicShipmentCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateSporadicShipmentCommandResult>> HandleAsync(
        CreateSporadicShipmentRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateSporadicShipmentCommand
        {
            UserId = currentUser.UserId!.Value,
            OriginDepartment = request.OriginDepartment,
            SenderFullName = request.SenderFullName,
            SenderPhone = request.SenderPhone,
            SenderAddress = request.SenderAddress,
            DestinationDepartment = request.DestinationDepartment,
            ClientPhone = request.ClientPhone,
            ClientFullName = request.ClientFullName,
            ClientAddress = request.ClientAddress,
            DeliveryType = request.DeliveryType,
            PackageCount = request.PackageCount,
            PackageDescription = request.PackageDescription,
            Lines = request.Lines.Select(l => new CreateSporadicShipmentLine
            {
                ArticleName = l.ArticleName,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                Weight = l.Weight,
                ShippingCost = l.ShippingCost
            }).ToList()
        };

        var result = await mediator.SendCommandAsync<CreateSporadicShipmentCommand, CreateSporadicShipmentCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/shipments/{result.Value.ShipmentId}", result.Value);
    }
}

public class CreateSporadicShipmentRequest
{
    public BolivianDepartment OriginDepartment { get; set; }
    public string SenderFullName { get; set; } = null!;
    public string SenderPhone { get; set; } = null!;
    public string SenderAddress { get; set; } = null!;

    public BolivianDepartment DestinationDepartment { get; set; }
    public string ClientPhone { get; set; } = null!;
    public string ClientFullName { get; set; } = null!;
    public string ClientAddress { get; set; } = null!;
    public DeliveryType DeliveryType { get; set; }

    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = null!;

    public List<CreateSporadicShipmentLineRequest> Lines { get; set; } = [];
}

public class CreateSporadicShipmentLineRequest
{
    public string ArticleName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
