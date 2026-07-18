using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Articles.Specifications;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandHandler(
    IRepository<Article> articleRepository,
    IRepository<Supplier> supplierRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateArticleCommand, UpdateArticleCommandResult>
{
    public async Task<Result<UpdateArticleCommandResult>> HandleAsync(
        UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        var article = await articleRepository.GetBySpecificationAsync(
            new ArticleByIdSpecification(command.Id), cancellationToken);

        if (article is null)
            return Result.Fail<UpdateArticleCommandResult>("Artículo no encontrado.", "article.notfound");

        var duplicate = await articleRepository.GetBySpecificationAsync(
            new ArticleBySkuSpecification(command.Sku), cancellationToken);

        if (duplicate is not null && duplicate.Id != article.Id)
            return Result.Fail<UpdateArticleCommandResult>(
                "Ya existe otro artículo con ese SKU.", "article.sku.duplicate");

        var supplier = await supplierRepository.GetBySpecificationAsync(
            new SupplierByIdSpecification(command.SupplierId), cancellationToken);

        if (supplier is null)
            return Result.Fail<UpdateArticleCommandResult>(
                "El proveedor indicado no existe.", "article.supplier.notfound");

        article.Sku = command.Sku;
        article.Name = command.Name;
        article.Count = command.Count;
        article.Price = command.Price;
        article.SupplierId = command.SupplierId;

        await articleRepository.UpdateAsync(article, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateArticleCommandResult
        {
            Id = article.Id,
            Sku = article.Sku,
            Name = article.Name,
            Count = article.Count,
            Price = article.Price,
            SupplierId = article.SupplierId
        });
    }
}
