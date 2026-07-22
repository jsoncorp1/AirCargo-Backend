using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveriesPaginated;

public class GetOrderDeliveriesPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<OrderDeliveryPaginatedItem> Data { get; set; } = [];
}

public class OrderDeliveryPaginatedItem
{
    public Guid Id { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string DeliveryType { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public bool IsAttended { get; set; }
    public DateTime CreatedAt { get; set; }
}
