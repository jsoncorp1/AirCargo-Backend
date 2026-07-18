using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Users.Commands.DeleteUser;

public class DeleteUserCommand : ICommand<DeleteUserCommandResult>
{
    public Guid Id { get; set; }
}