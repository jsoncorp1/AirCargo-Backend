using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Shipments.Commands.UpdateShipment;

public class UpdateShipmentCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryId { get; set; }
    public string WaybillNumber { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }
    public List<UpdateShipmentDetailResult> Details { get; set; } = [];
}

public class UpdateShipmentDetailResult
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
