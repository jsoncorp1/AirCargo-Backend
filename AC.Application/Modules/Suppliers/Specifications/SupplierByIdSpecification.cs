using AC.Domain.Modules.Suppliers;
using Ardalis.Specification;

namespace AC.Application.Modules.Suppliers.Specifications;

public class SupplierByIdSpecification : Specification<Supplier>
{
    public SupplierByIdSpecification(Guid id) => Query.Where(s => s.Id == id);
}
