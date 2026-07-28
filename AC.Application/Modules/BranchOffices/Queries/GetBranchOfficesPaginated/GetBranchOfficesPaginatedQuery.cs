using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.BranchOffices.Queries.GetBranchOfficesPaginated;

public class GetBranchOfficesPaginatedQuery : IQuery<GetBranchOfficesPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}
