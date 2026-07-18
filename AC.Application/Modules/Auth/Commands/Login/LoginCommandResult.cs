using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Auth.Commands.Login;

public class LoginCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string Token { get; set; } = string.Empty;
}