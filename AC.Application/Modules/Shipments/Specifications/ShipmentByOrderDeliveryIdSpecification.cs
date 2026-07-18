using AC.Domain.Modules.Shipments;
using Ardalis.Specification;

namespace AC.Application.Modules.Shipments.Specifications;

public class ShipmentByOrderDeliveryIdSpecification : Specification<Shipment>
{
    public ShipmentByOrderDeliveryIdSpecification(Guid orderDeliveryId) =>
        Query.Where(s => s.OrderDeliveryId == orderDeliveryId);
}
