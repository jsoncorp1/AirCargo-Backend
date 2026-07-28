using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Articles.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Articles.Queries.GetArticleById;

public class GetArticleByIdQueryHandler(
    IRepository<Article> repository,
    IRepository<User> userRepository)
    : IQueryHandler<GetArticleByIdQuery, GetArticleByIdQueryResult>
{
    public async Task<Result<GetArticleByIdQueryResult>> HandleAsync(
        GetArticleByIdQuery query, CancellationToken cancellationToken)
    {
        var article = await repository.GetBySpecificationAsync(
            new ArticleByIdSpecification(query.Id), cancellationToken);

        if (article is null)
            return Result.Fail<GetArticleByIdQueryResult>("Artículo no encontrado.", "article.notfound");

        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(query.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<GetArticleByIdQueryResult>(
                "El usuario autenticado no existe.", "article.user.notfound");

        if (actor.Role.Name == RoleNames.UsuarioEmpresa && article.SupplierId != actor.SupplierId)
            return Result.Fail<GetArticleByIdQueryResult>(
                "El artículo no pertenece al proveedor del usuario.", "article.access.forbidden");

        return Result.Success(new GetArticleByIdQueryResult
        {
            Id = article.Id,
            Sku = article.Sku,
            Name = article.Name,
            Count = article.Count,
            Price = article.Price,
            SupplierId = article.SupplierId,
            SupplierName = article.Supplier.Name
        });
    }
}
