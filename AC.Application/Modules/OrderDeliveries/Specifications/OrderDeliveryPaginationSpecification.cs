using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.OrderDeliveries.Specifications;

public class OrderDeliveryPaginationSpecification : PaginationSpecification<OrderDelivery>
{
    public OrderDeliveryPaginationSpecification(
        int page, int perPage, Guid? supplierId = null, bool? unattended = null,
        BolivianDepartment? originDepartment = null)
        : base(page, perPage)
    {
        Query
            .Include(o => o.Supplier)
            .Include(o => o.Shipments.Where(s => s.Active))
            .OrderByDescending(o => o.CreatedAt);

        if (supplierId is not null)
            Query.Where(o => o.SupplierId == supplierId);

        if (unattended is true)
            Query.Where(o => !o.Shipments.Any(s => s.Active));

        // Alcance del admin: las órdenes que nacen en el departamento de su sucursal.
        if (originDepartment is not null)
            Query.Where(o => o.OriginDepartment == originDepartment);
    }
}
