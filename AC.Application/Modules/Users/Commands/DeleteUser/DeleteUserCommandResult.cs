using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Users.Commands.DeleteUser;

public class DeleteUserCommandResult : ICommandResult
{
    public Guid Id { get; set; }
}