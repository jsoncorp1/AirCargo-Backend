using AC.Domain.Common;
using AC.Domain.Modules.Users;

namespace AC.Domain.Modules.Roles;

public class Role : CoreEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<User> Users { get; } = new List<User>(); 
}