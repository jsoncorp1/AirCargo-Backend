using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.OrderDeliveries.Commands.UpdateOrderDelivery;

public class UpdateOrderDeliveryCommand : ICommand<UpdateOrderDeliveryCommandResult>
{
    public Guid Id { get; set; }
    public BolivianDepartment DestinationDepartment { get; set; }
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public DeliveryType DeliveryType { get; set; }
    public List<UpdateOrderDeliveryLine> Lines { get; set; } = [];
}

public class UpdateOrderDeliveryLine
{
    public Guid ArticleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
