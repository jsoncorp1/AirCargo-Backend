using AC.Domain.Modules.Shipments;
using Ardalis.Specification;

namespace AC.Application.Modules.Shipments.Specifications;

public class ShipmentByIdSpecification : Specification<Shipment>
{
    public ShipmentByIdSpecification(Guid id)
    {
        Query
            .Where(s => s.Id == id)
            .Include(s => s.OrderDelivery)
            .Include(s => s.ShipmentDetails.Where(d => d.Active))
                .ThenInclude(d => d.OrderDeliveryDetail);
    }
}
