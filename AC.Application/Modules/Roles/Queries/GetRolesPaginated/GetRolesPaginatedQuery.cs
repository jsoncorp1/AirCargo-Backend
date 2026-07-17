using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Roles.Queries.GetRolesPaginated;

public class GetRolesPaginatedQuery : IQuery<GetRolesPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}