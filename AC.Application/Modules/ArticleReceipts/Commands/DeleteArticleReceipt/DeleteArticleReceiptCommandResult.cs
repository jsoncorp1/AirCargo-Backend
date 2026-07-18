using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.ArticleReceipts.Commands.DeleteArticleReceipt;

public class DeleteArticleReceiptCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public int ArticleTotalCount { get; set; }
}
