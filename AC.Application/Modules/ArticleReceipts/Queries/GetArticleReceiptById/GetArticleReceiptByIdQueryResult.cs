using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptById;

public class GetArticleReceiptByIdQueryResult : IQueryResult
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string ArticleSku { get; set; } = string.Empty;
    public string ArticleName { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime CreatedAt { get; set; }
}
