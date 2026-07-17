using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Roles.Commands.UpdateRole;

public class UpdateRoleCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}