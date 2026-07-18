using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Shipments.Commands.CreateShipment;

public class CreateShipmentCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryId { get; set; }
    public string NumeroGuia { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }
    public List<CreateShipmentDetailResult> Details { get; set; } = [];
}

public class CreateShipmentDetailResult
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryDetailId { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
