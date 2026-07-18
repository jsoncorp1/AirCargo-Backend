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
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string TipoEntrega { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public bool IsAttended { get; set; }
    public DateTime CreatedAt { get; set; }
}
