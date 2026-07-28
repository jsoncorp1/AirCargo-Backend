using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptsPaginated;

public class GetArticleReceiptsPaginatedQuery : IQuery<GetArticleReceiptsPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;

    // Usuario autenticado que consulta; define el alcance según su rol.
    public Guid UserId { get; set; }

    public Guid? ArticleId { get; set; }
}
