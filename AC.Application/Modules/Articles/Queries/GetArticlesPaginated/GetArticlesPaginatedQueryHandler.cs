using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Articles.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Articles.Queries.GetArticlesPaginated;

public class GetArticlesPaginatedQueryHandler(
    IRepository<Article> repository,
    IRepository<User> userRepository)
    : IQueryHandler<GetArticlesPaginatedQuery, GetArticlesPaginatedQueryResult>
{
    public async Task<Result<GetArticlesPaginatedQueryResult>> HandleAsync(
        GetArticlesPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(query.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<GetArticlesPaginatedQueryResult>(
                "El usuario autenticado no existe.", "article.user.notfound");

        // usuarioempresa solo ve artículos de su proveedor; se ignora el filtro del request.
        var supplierId = query.SupplierId;
        if (actor.Role.Name == RoleNames.UsuarioEmpresa)
        {
            if (actor.SupplierId is null)
                return Result.Fail<GetArticlesPaginatedQueryResult>(
                    "El usuario no pertenece a ningún proveedor.", "article.user.notsupplier");

            supplierId = actor.SupplierId;
        }

        var spec = new ArticlePaginationSpecification(page, perPage, supplierId, query.ArticleName);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetArticlesPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(a => new ArticlePaginatedItem
            {
                Id = a.Id,
                Sku = a.Sku,
                Name = a.Name,
                Count = a.Count,
                Price = a.Price,
                SupplierId = a.SupplierId,
                SupplierName = a.Supplier.Name
            })
        });
    }
}
