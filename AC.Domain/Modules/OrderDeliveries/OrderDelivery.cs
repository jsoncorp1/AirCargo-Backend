using AC.Domain.Common;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Modules.Users;

namespace AC.Domain.Modules.OrderDeliveries;

public class OrderDelivery : CoreEntity
{
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DepartamentoBolivia Departamento { get; set; }
    public string ClientePhone { get; set; } = string.Empty;
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public string ClienteDireccion { get; set; } = string.Empty;
    public TipoEntrega TipoEntrega { get; set; }

    public decimal TotalPrice { get; set; }

    public ICollection<OrderDeliveryDetail> OrderDeliveryDetails { get; set; } = new List<OrderDeliveryDetail>();

    // Históricamente puede haber más de un Shipment (uno cancelado y otro nuevo);
    // la regla de "a lo sumo un envío activo por orden" la aplica el índice único
    // parcial (WHERE active) y la validación de negocio, no la cardinalidad EF.
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
