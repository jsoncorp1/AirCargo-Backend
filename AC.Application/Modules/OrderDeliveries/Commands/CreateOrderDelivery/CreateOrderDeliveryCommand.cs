using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.OrderDeliveries.Commands.CreateOrderDelivery;

public class CreateOrderDeliveryCommand : ICommand<CreateOrderDeliveryCommandResult>
{
    public Guid UserId { get; set; }
    public DepartamentoBolivia Departamento { get; set; }
    public string ClientePhone { get; set; } = string.Empty;
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public string ClienteDireccion { get; set; } = string.Empty;
    public TipoEntrega TipoEntrega { get; set; }
    public List<CreateOrderDeliveryLine> Lines { get; set; } = [];
}

public class CreateOrderDeliveryLine
{
    public Guid ArticleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
