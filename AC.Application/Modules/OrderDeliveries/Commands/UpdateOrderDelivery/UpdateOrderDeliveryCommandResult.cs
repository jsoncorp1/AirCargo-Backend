using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.OrderDeliveries.Commands.UpdateOrderDelivery;

public class UpdateOrderDeliveryCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public Guid UserId { get; set; }
    public DepartamentoBolivia Departamento { get; set; }
    public string ClientePhone { get; set; } = string.Empty;
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public string ClienteDireccion { get; set; } = string.Empty;
    public TipoEntrega TipoEntrega { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderDeliveryDetailResult> Details { get; set; } = [];
}

public class OrderDeliveryDetailResult
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
