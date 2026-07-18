using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Articles.Queries.GetArticlesPaginated;

public class GetArticlesPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<ArticlePaginatedItem> Data { get; set; } = [];
}

public class ArticlePaginatedItem
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Price { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
}
