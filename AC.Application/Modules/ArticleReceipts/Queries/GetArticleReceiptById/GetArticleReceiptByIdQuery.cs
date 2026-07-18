using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptById;

public class GetArticleReceiptByIdQuery : IQuery<GetArticleReceiptByIdQueryResult>
{
    public Guid Id { get; set; }
}
