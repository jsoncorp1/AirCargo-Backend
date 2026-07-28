using AC.Application.Abstractions.Messaging.Queries;
using AC.Domain.Modules.Shipments;

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
    public string Code { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public Guid? SupplierId { get; set; }
    public Guid? OriginBranchOfficeId { get; set; }
    public string? OriginBranchOfficeCode { get; set; }
    public Guid? DestinationBranchOfficeId { get; set; }
    public string? DestinationBranchOfficeCode { get; set; }
    public ShipmentStatus Status { get; set; }
    public ShipmentObservation? Observation { get; set; }
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }
    public int PackageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
