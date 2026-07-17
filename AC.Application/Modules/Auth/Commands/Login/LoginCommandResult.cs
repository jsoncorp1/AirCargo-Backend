using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Auth.Commands.Login;

public class LoginCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}