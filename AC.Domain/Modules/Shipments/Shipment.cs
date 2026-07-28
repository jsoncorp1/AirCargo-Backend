using AC.Domain.Common;
using AC.Domain.Modules.BranchOffices;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Domain.Modules.Shipments;

public class Shipment : CoreEntity
{
    public Guid OrderDeliveryId { get; set; }
    public OrderDelivery OrderDelivery { get; set; } = null!;

    // Sucursales del envío: origen = sucursal del usuario que atiende la orden,
    // destino se selecciona al atender. Nullable solo por los envíos históricos
    // anteriores al modelo multisucursal; todo envío nuevo las lleva siempre.
    public Guid? OriginBranchOfficeId { get; set; }
    public BranchOffice? OriginBranchOffice { get; set; }

    public Guid? DestinationBranchOfficeId { get; set; }
    public BranchOffice? DestinationBranchOffice { get; set; }

    public int SequenceNumber { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }

    // Cómo viene agrupado físicamente el envío (ej. "3 cajas y 2 sobres"),
    // independiente de cuántas líneas/artículos tenga el detalle.
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;

    // Estado del reparto; las transiciones válidas las define ShipmentStatusRules.
    public ShipmentStatus Status { get; set; } = ShipmentStatus.InTransit;

    // Última observación registrada por el delivery; al setearla el estado
    // pasa automáticamente a Observed (o Rejected si es CustomerRefused).
    public ShipmentObservation? Observation { get; set; }

    // Comentario libre del delivery sobre el intento de entrega.
    public string? DeliveryComment { get; set; }

    public ICollection<ShipmentDetail> ShipmentDetails { get; set; } = new List<ShipmentDetail>();
}
