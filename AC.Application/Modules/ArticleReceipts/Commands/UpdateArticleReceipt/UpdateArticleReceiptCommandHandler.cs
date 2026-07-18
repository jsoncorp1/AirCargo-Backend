using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.ArticleReceipts.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.ArticleReceipts.Commands.UpdateArticleReceipt;

public class UpdateArticleReceiptCommandHandler(
    IRepository<ArticleReceipt> receiptRepository,
    IRepository<Article> articleRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateArticleReceiptCommand, UpdateArticleReceiptCommandResult>
{
    public async Task<Result<UpdateArticleReceiptCommandResult>> HandleAsync(
        UpdateArticleReceiptCommand command, CancellationToken cancellationToken)
    {
        if (command.Count <= 0)
            return Result.Fail<UpdateArticleReceiptCommandResult>(
                "La cantidad recibida debe ser mayor a cero.", "articlereceipt.count.invalid");

        var receipt = await receiptRepository.GetBySpecificationAsync(
            new ArticleReceiptByIdSpecification(command.Id), cancellationToken);

        if (receipt is null)
            return Result.Fail<UpdateArticleReceiptCommandResult>(
                "Recepción no encontrada.", "articlereceipt.notfound");

        int delta = command.Count - receipt.Count;
        int newTotal = receipt.Article.Count + delta;

        if (newTotal < 0)
            return Result.Fail<UpdateArticleReceiptCommandResult>(
                "El ajuste dejaría el stock del artículo en negativo.", "articlereceipt.count.invalid");

        receipt.Count = command.Count;
        receipt.Article.Count = newTotal;

        await receiptRepository.UpdateAsync(receipt, cancellationToken);
        await articleRepository.UpdateAsync(receipt.Article, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateArticleReceiptCommandResult
        {
            Id = receipt.Id,
            ArticleId = receipt.ArticleId,
            Count = receipt.Count,
            ArticleTotalCount = receipt.Article.Count
        });
    }
}
