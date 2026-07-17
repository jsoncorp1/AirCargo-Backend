using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Roles.Queries.GetRolesPaginated;

public class GetRolesPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<RolePaginatedItem> Data { get; set; } = [];
}

public class RolePaginatedItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}