using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.ArticleReceipts.Commands.CreateArticleReceipt;

public class CreateArticleReceiptCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public int Count { get; set; }
    public int ArticleTotalCount { get; set; }
}
