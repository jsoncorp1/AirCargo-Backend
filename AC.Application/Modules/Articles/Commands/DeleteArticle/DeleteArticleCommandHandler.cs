using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Articles.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandHandler(
    IRepository<Article> repository,
    IRepository<Supplier> supplierRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteArticleCommand, DeleteArticleCommandResult>
{
    public async Task<Result<DeleteArticleCommandResult>> HandleAsync(
        DeleteArticleCommand command, CancellationToken cancellationToken)
    {
        var article = await repository.GetBySpecificationAsync(
            new ArticleByIdSpecification(command.Id), cancellationToken);

        if (article is null)
            return Result.Fail<DeleteArticleCommandResult>("Artículo no encontrado.", "article.notfound");

        article.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy
        article.Supplier.ArticleQuantity = Math.Max(0, article.Supplier.ArticleQuantity - 1);

        await repository.UpdateAsync(article, cancellationToken);
        await supplierRepository.UpdateAsync(article.Supplier, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteArticleCommandResult { Id = article.Id });
    }
}
