using AC.Domain.Common;
using AC.Domain.Modules.Articles;

namespace AC.Domain.Modules.OrderDeliveries;

public class OrderDeliveryDetail : CoreEntity
{
    public Guid OrderDeliveryId { get; set; }
    public OrderDelivery OrderDelivery { get; set; } = null!;

    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public string ArticleName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
