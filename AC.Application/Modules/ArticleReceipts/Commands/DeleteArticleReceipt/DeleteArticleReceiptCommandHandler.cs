using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.ArticleReceipts.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.ArticleReceipts.Commands.DeleteArticleReceipt;

public class DeleteArticleReceiptCommandHandler(
    IRepository<ArticleReceipt> receiptRepository,
    IRepository<Article> articleRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteArticleReceiptCommand, DeleteArticleReceiptCommandResult>
{
    public async Task<Result<DeleteArticleReceiptCommandResult>> HandleAsync(
        DeleteArticleReceiptCommand command, CancellationToken cancellationToken)
    {
        var receipt = await receiptRepository.GetBySpecificationAsync(
            new ArticleReceiptByIdSpecification(command.Id), cancellationToken);

        if (receipt is null)
            return Result.Fail<DeleteArticleReceiptCommandResult>(
                "Recepción no encontrada.", "articlereceipt.notfound");

        receipt.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy
        receipt.Article.Count = Math.Max(0, receipt.Article.Count - receipt.Count);

        await receiptRepository.UpdateAsync(receipt, cancellationToken);
        await articleRepository.UpdateAsync(receipt.Article, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteArticleReceiptCommandResult
        {
            Id = receipt.Id,
            ArticleTotalCount = receipt.Article.Count
        });
    }
}
