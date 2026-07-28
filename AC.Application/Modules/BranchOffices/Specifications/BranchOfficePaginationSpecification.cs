using AC.Domain.Modules.BranchOffices;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.BranchOffices.Specifications;

public class BranchOfficePaginationSpecification : PaginationSpecification<BranchOffice>
{
    public BranchOfficePaginationSpecification(int page, int perPage) : base(page, perPage)
    {
        Query.OrderBy(b => b.Code);
    }
}
