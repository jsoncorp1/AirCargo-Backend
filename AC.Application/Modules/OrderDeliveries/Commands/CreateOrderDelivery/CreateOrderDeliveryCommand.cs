using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.OrderDeliveries.Commands.CreateOrderDelivery;

public class CreateOrderDeliveryCommand : ICommand<CreateOrderDeliveryCommandResult>
{
    public Guid UserId { get; set; }
    public BolivianDepartment DestinationDepartment { get; set; }
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public DeliveryType DeliveryType { get; set; }
    public List<CreateOrderDeliveryLine> Lines { get; set; } = [];
}

public class CreateOrderDeliveryLine
{
    public Guid ArticleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
