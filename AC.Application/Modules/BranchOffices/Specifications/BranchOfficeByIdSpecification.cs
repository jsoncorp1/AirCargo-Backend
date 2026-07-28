using AC.Domain.Modules.BranchOffices;
using Ardalis.Specification;

namespace AC.Application.Modules.BranchOffices.Specifications;

public class BranchOfficeByIdSpecification : Specification<BranchOffice>
{
    public BranchOfficeByIdSpecification(Guid id) => Query.Where(b => b.Id == id);
}
