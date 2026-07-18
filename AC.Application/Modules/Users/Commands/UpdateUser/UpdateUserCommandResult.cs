using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Users.Commands.UpdateUser;

public class UpdateUserCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}