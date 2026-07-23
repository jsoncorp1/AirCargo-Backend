using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.OrderDeliveries.Commands.UpdateOrderDelivery;

public class UpdateOrderDeliveryCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid UserId { get; set; }
    public BolivianDepartment DestinationDepartment { get; set; }
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public DeliveryType DeliveryType { get; set; }
    public decimal TotalPrice { get; set; }
    public List<UpdateOrderDeliveryDetailResult> Details { get; set; } = [];
}

public class UpdateOrderDeliveryDetailResult
{
    public Guid Id { get; set; }
    public Guid? ArticleId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
