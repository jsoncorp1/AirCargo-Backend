using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.OrderDeliveries.Specifications;

public class OrderDeliveryPaginationSpecification : PaginationSpecification<OrderDelivery>
{
    public OrderDeliveryPaginationSpecification(
        int page, int perPage, Guid? supplierId = null, bool? unattended = null)
        : base(page, perPage)
    {
        Query
            .Include(o => o.Supplier)
            .Include(o => o.Shipment)
            .OrderByDescending(o => o.CreatedAt);

        if (supplierId is not null)
            Query.Where(o => o.SupplierId == supplierId);

        if (unattended is true)
            Query.Where(o => o.Shipment == null || !o.Shipment.Active);
    }
}
