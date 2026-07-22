using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Suppliers.Specifications;

public class SupplierPaginationSpecification : PaginationSpecification<Supplier>
{
    public SupplierPaginationSpecification(
        int page, int perPage, BolivianDepartment? department = null) : base(page, perPage)
    {
        Query.OrderBy(s => s.Name);

        if (department is not null)
            Query.Where(s => s.Department == department);
    }
}
