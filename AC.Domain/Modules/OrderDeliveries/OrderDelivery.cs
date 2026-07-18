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
    public Shipment? Shipment { get; set; }
}
