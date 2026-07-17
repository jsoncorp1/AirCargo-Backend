using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Auth.Commands.Login;

public class LoginCommand : ICommand<LoginCommandResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}