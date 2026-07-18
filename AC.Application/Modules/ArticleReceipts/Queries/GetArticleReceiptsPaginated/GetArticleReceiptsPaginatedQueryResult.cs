using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptsPaginated;

public class GetArticleReceiptsPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<ArticleReceiptPaginatedItem> Data { get; set; } = [];
}

public class ArticleReceiptPaginatedItem
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public string ArticleSku { get; set; } = string.Empty;
    public string ArticleName { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime CreatedAt { get; set; }
}
