using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveryById;

public class GetOrderDeliveryByIdQueryResult : IQueryResult
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public string DeliveryType { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public bool IsAttended { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderDeliveryDetailItem> Details { get; set; } = [];
}

public class OrderDeliveryDetailItem
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
