using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.ArticleReceipts.Commands.CreateArticleReceipt;

public class CreateArticleReceiptCommand : ICommand<CreateArticleReceiptCommandResult>
{
    public Guid ArticleId { get; set; }
    public int Count { get; set; }
}
