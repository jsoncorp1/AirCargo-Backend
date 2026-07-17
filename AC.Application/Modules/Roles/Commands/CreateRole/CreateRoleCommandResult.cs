using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Roles.Commands.CreateRole;

public class CreateRoleCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}