using AC.Domain.Common;
using AC.Domain.Modules.Roles;

namespace AC.Domain.Modules.Users;

public class User : CoreEntity
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public Guid RoleId { get; set; }         
    public Role Role { get; set; } = null!;  
}