using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : ICommand<DeleteRoleCommandResult>
{
    public Guid Id { get; set; }
}