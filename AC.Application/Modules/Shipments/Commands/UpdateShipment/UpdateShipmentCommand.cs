using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Shipments.Commands.UpdateShipment;

public class UpdateShipmentCommand : ICommand<UpdateShipmentCommandResult>
{
    public Guid Id { get; set; }
    public List<UpdateShipmentLine> Lines { get; set; } = [];
}

public class UpdateShipmentLine
{
    public Guid ShipmentDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
