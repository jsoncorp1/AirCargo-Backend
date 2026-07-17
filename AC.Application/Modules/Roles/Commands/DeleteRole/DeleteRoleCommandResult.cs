using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Roles.Commands.DeleteRole;

public class DeleteRoleCommandResult : ICommandResult
{
    public Guid Id { get; set; }
}