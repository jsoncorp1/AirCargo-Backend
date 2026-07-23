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

    // Cómo viene agrupado físicamente el envío (ej. "3 cajas y 2 sobres"),
    // independiente de cuántas líneas/artículos tenga el detalle.
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;

    public ICollection<ShipmentDetail> ShipmentDetails { get; set; } = new List<ShipmentDetail>();
}
