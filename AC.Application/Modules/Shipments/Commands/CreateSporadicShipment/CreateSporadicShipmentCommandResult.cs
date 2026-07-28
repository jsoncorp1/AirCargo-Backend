using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.Shipments;

namespace AC.Application.Modules.Shipments.Commands.CreateSporadicShipment;

public class CreateSporadicShipmentCommandResult : ICommandResult
{
    public Guid OrderDeliveryId { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid? OriginBranchOfficeId { get; set; }
    public Guid? DestinationBranchOfficeId { get; set; }
    public ShipmentStatus Status { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsExpress { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;
    public List<CreateSporadicShipmentDetailResult> Details { get; set; } = [];
}

public class CreateSporadicShipmentDetailResult
{
    public Guid OrderDeliveryDetailId { get; set; }
    public Guid ShipmentDetailId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
