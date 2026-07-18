using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.ArticleReceipts.Commands.DeleteArticleReceipt;

public class DeleteArticleReceiptCommand : ICommand<DeleteArticleReceiptCommandResult>
{
    public Guid Id { get; set; }
}
