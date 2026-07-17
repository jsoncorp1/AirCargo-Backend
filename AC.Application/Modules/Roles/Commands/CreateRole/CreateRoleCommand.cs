using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Roles.Commands.CreateRole;

public class CreateRoleCommand : ICommand<CreateRoleCommandResult>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}