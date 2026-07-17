using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Roles.Commands.UpdateRole;

public class UpdateRoleCommand : ICommand<UpdateRoleCommandResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}