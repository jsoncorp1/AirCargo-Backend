using AC.Domain.Common;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Domain.Modules.Shipments;

public class Shipment : CoreEntity
{
    public Guid OrderDeliveryId { get; set; }
    public OrderDelivery OrderDelivery { get; set; } = null!;

    public int SequenceNumber { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }

    public ICollection<ShipmentDetail> ShipmentDetails { get; set; } = new List<ShipmentDetail>();
}
