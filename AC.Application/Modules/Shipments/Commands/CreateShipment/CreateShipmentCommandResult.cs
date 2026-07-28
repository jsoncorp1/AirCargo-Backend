using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.Shipments;

namespace AC.Application.Modules.Shipments.Commands.CreateShipment;

public class CreateShipmentCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryId { get; set; }
    public Guid? OriginBranchOfficeId { get; set; }
    public Guid? DestinationBranchOfficeId { get; set; }
    public ShipmentStatus Status { get; set; }
    public string WaybillNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;
    public List<CreateShipmentDetailResult> Details { get; set; } = [];
}

public class CreateShipmentDetailResult
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
