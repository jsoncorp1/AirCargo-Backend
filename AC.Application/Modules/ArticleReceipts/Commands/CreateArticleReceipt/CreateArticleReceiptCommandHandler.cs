using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Articles.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.ArticleReceipts.Commands.CreateArticleReceipt;

public class CreateArticleReceiptCommandHandler(
    IRepository<ArticleReceipt> receiptRepository,
    IRepository<Article> articleRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateArticleReceiptCommand, CreateArticleReceiptCommandResult>
{
    public async Task<Result<CreateArticleReceiptCommandResult>> HandleAsync(
        CreateArticleReceiptCommand command, CancellationToken cancellationToken)
    {
        if (command.Count <= 0)
            return Result.Fail<CreateArticleReceiptCommandResult>(
                "La cantidad recibida debe ser mayor a cero.", "articlereceipt.count.invalid");

        var article = await articleRepository.GetBySpecificationAsync(
            new ArticleByIdSpecification(command.ArticleId), cancellationToken);

        if (article is null)
            return Result.Fail<CreateArticleReceiptCommandResult>(
                "El artículo indicado no existe.", "articlereceipt.article.notfound");

        var receipt = new ArticleReceipt
        {
            Id = Guid.NewGuid(),
            ArticleId = command.ArticleId,
            Count = command.Count
        };

        article.Count += command.Count;

        await receiptRepository.SaveAsync(receipt, cancellationToken);
        await articleRepository.UpdateAsync(article, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateArticleReceiptCommandResult
        {
            Id = receipt.Id,
            ArticleId = receipt.ArticleId,
            Count = receipt.Count,
            ArticleTotalCount = article.Count
        });
    }
}
