using AC.Domain.Common;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.Users;

namespace AC.Domain.Modules.Suppliers;

public class Supplier : CoreEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<User> Users { get; } = new List<User>(); 
    
    public ICollection<Article> Articles { get; } = new List<Article>();
}