using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentsPaginated;

public class GetShipmentsPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<ShipmentPaginatedItem> Data { get; set; } = [];
}

public class ShipmentPaginatedItem
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryId { get; set; }
    public string WaybillNumber { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
