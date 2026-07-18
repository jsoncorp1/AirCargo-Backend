using AC.Domain.Common;
using AC.Domain.Modules.Suppliers;

namespace AC.Domain.Modules.Articles;

public class Article : CoreEntity
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; } 
    public decimal Price { get; set; }
    
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
}