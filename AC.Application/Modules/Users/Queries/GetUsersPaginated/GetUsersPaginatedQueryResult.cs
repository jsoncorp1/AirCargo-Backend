using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<UserPaginatedItem> Data { get; set; } = [];
}

public class UserPaginatedItem
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}