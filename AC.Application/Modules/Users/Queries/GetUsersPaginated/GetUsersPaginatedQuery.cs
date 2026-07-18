using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQuery : IQuery<GetUsersPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? RoleId { get; set; }
    public Guid? SupplierId { get; set; }
}