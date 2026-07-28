using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Users.Queries.GetUserById;

public class GetUserByIdQuery : IQuery<GetUserByIdQueryResult>
{
    public Guid Id { get; set; }

    // Usuario autenticado que consulta; admin solo ve conductores de su sucursal.
    public Guid UserId { get; set; }
}