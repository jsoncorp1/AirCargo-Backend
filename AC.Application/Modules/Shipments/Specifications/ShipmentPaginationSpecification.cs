using AC.Domain.Modules.Shipments;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Shipments.Specifications;

public class ShipmentPaginationSpecification : PaginationSpecification<Shipment>
{
    public ShipmentPaginationSpecification(int page, int perPage, Guid? supplierId = null)
        : base(page, perPage)
    {
        Query
            .Include(s => s.OrderDelivery)
            .OrderByDescending(s => s.CreatedAt);

        if (supplierId is not null)
            Query.Where(s => s.OrderDelivery.SupplierId == supplierId);
    }
}
