using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Articles.Specifications;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Articles.Commands.CreateArticle;

public class CreateArticleCommandHandler(
    IRepository<Article> articleRepository,
    IRepository<Supplier> supplierRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateArticleCommand, CreateArticleCommandResult>
{
    public async Task<Result<CreateArticleCommandResult>> HandleAsync(
        CreateArticleCommand command, CancellationToken cancellationToken)
    {
        Result validation = await ValidateAsync(command, cancellationToken);
        if (validation.Failure)
            return Result.Fail<CreateArticleCommandResult>(validation.Error, validation.ErrorKey);

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Sku = command.Sku,
            Name = command.Name,
            Count = 0,
            Price = command.Price,
            SupplierId = command.SupplierId
        };

        await articleRepository.SaveAsync(article, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateArticleCommandResult
        {
            Id = article.Id,
            Sku = article.Sku,
            Name = article.Name,
            Count = article.Count,
            Price = article.Price,
            SupplierId = article.SupplierId
        });
    }

    private async Task<Result> ValidateAsync(
        CreateArticleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Sku))
            return Result.Fail("El SKU es obligatorio.", "article.sku.required");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Fail("El nombre es obligatorio.", "article.name.required");

        if (command.SupplierId == Guid.Empty)
            return Result.Fail("El proveedor es obligatorio.", "article.supplierid.required");

        var existingSku = await articleRepository.GetBySpecificationAsync(
            new ArticleBySkuSpecification(command.Sku), cancellationToken);

        if (existingSku is not null)
            return Result.Fail("Ya existe un artículo con ese SKU.", "article.sku.duplicate");

        var supplier = await supplierRepository.GetBySpecificationAsync(
            new SupplierByIdSpecification(command.SupplierId), cancellationToken);

        if (supplier is null)
            return Result.Fail("El proveedor indicado no existe.", "article.supplier.notfound");

        return Result.Success();
    }
}
