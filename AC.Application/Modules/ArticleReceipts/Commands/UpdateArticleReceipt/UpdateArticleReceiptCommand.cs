using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.ArticleReceipts.Commands.UpdateArticleReceipt;

public class UpdateArticleReceiptCommand : ICommand<UpdateArticleReceiptCommandResult>
{
    public Guid Id { get; set; }
    public int Count { get; set; }
}
