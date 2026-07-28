using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Users.Commands.DeleteUser;

public class DeleteUserCommand : ICommand<DeleteUserCommandResult>
{
    public Guid Id { get; set; }

    // Usuario autenticado que elimina; admin solo puede eliminar conductores de su sucursal.
    public Guid ActorUserId { get; set; }
}