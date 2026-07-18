using AC.Domain.Modules.OrderDeliveries;
using Ardalis.Specification;

namespace AC.Application.Modules.OrderDeliveries.Specifications;

public class OrderDeliveryByIdSpecification : Specification<OrderDelivery>
{
    public OrderDeliveryByIdSpecification(Guid id)
    {
        Query
            .Where(o => o.Id == id)
            .Include(o => o.Supplier)
            .Include(o => o.User)
            .Include(o => o.OrderDeliveryDetails.Where(d => d.Active))
            .Include(o => o.Shipments.Where(s => s.Active));
    }
}
