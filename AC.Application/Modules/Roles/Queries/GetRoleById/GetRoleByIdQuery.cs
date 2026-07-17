using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Roles.Queries.GetRoleById;

public class GetRoleByIdQuery : IQuery<GetRoleByIdQueryResult>
{
    public Guid Id { get; set; }
}