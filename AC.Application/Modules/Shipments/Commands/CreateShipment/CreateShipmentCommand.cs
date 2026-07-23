using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Shipments.Commands.CreateShipment;

public class CreateShipmentCommand : ICommand<CreateShipmentCommandResult>
{
    public Guid OrderDeliveryId { get; set; }
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;
    public List<CreateShipmentLine> Lines { get; set; } = [];
}

public class CreateShipmentLine
{
    public Guid OrderDeliveryDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
