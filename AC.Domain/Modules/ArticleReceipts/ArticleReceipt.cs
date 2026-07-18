using AC.Domain.Common;
using AC.Domain.Modules.Articles;

namespace AC.Domain.Modules.ArticleReceipts;

public class ArticleReceipt : CoreEntity
{
    public int Count { get; set; }
    
    public Guid ArticleId { get; set; }         
    public Article Article { get; set; } = null!;  
}