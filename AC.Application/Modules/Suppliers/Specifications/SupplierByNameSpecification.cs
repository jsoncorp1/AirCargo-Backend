using AC.Domain.Modules.Suppliers;
using Ardalis.Specification;

namespace AC.Application.Modules.Suppliers.Specifications;

public class SupplierByNameSpecification : Specification<Supplier>
{
    public SupplierByNameSpecification(string name) => Query.Where(s => s.Name == name);
}
