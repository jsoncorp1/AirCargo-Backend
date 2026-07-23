using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Shipments.Commands.UpdateShipment;

public class UpdateShipmentCommand : ICommand<UpdateShipmentCommandResult>
{
    public Guid Id { get; set; }
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;
    public List<UpdateShipmentLine> Lines { get; set; } = [];
}

public class UpdateShipmentLine
{
    public Guid ShipmentDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
