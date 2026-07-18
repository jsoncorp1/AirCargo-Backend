using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Articles.Queries.GetArticlesPaginated;

public class GetArticlesPaginatedQuery : IQuery<GetArticlesPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? SupplierId { get; set; }
}
