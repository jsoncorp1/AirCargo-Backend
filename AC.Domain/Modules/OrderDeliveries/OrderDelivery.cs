using AC.Domain.Common;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Modules.Users;

namespace AC.Domain.Modules.OrderDeliveries;

public class OrderDelivery : CoreEntity
{
    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public OrderType OrderType { get; set; }

    // Origen del envío: en corporativo es un snapshot de Supplier.Department/Name
    // tomado al crear la orden (no navega Supplier en caliente, así una guía
    // histórica no cambia si el proveedor se muda o se renombra después);
    // en esporádico lo carga el mostrador a mano.
    public BolivianDepartment OriginDepartment { get; set; }
    public string SenderFullName { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;

    // Destino y datos del destinatario (antes "Department"/"Client*").
    public BolivianDepartment DestinationDepartment { get; set; }
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public DeliveryType DeliveryType { get; set; }

    public decimal TotalPrice { get; set; }

    public ICollection<OrderDeliveryDetail> OrderDeliveryDetails { get; set; } = new List<OrderDeliveryDetail>();

    // Históricamente puede haber más de un Shipment (uno cancelado y otro nuevo);
    // la regla de "a lo sumo un envío activo por orden" la aplica el índice único
    // parcial (WHERE active) y la validación de negocio, no la cardinalidad EF.
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
