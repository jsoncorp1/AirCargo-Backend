using AC.Domain.Common;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Domain.Modules.Shipments;

public class ShipmentDetail : CoreEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = null!;

    public Guid OrderDeliveryDetailId { get; set; }
    public OrderDeliveryDetail OrderDeliveryDetail { get; set; } = null!;

    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
