using AC.Application.Abstractions.Messaging.Queries;
using AC.Domain.Modules.Shipments;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentById;

public class GetShipmentByIdQueryResult : IQueryResult
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryId { get; set; }
    public string WaybillNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string OriginDepartment { get; set; } = string.Empty;
    public string SenderFullName { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public string DestinationDepartment { get; set; } = string.Empty;
    public Guid? OriginBranchOfficeId { get; set; }
    public string? OriginBranchOfficeCode { get; set; }
    public string? OriginBranchOfficeCity { get; set; }
    public Guid? DestinationBranchOfficeId { get; set; }
    public string? DestinationBranchOfficeCode { get; set; }
    public string? DestinationBranchOfficeCity { get; set; }
    public ShipmentStatus Status { get; set; }
    public ShipmentObservation? Observation { get; set; }
    public string? DeliveryComment { get; set; }
    public decimal TotalWeight { get; set; }
    public decimal ShippingPrice { get; set; }
    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<ShipmentDetailItem> Details { get; set; } = [];
}

public class ShipmentDetailItem
{
    public Guid Id { get; set; }
    public Guid OrderDeliveryDetailId { get; set; }
    public string ArticleName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
