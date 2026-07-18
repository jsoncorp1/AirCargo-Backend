using AC.Domain.Modules.Suppliers;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Suppliers.Specifications;

public class SupplierPaginationSpecification : PaginationSpecification<Supplier>
{
    public SupplierPaginationSpecification(int page, int perPage) : base(page, perPage)
    {
        Query.OrderBy(s => s.Name);
    }
}
