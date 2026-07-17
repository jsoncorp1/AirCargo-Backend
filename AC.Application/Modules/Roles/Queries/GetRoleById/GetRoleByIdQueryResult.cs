using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryResult : IQueryResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}