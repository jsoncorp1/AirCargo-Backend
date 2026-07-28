using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.ArticleReceipts.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptById;

public class GetArticleReceiptByIdQueryHandler(
    IRepository<ArticleReceipt> repository,
    IRepository<User> userRepository)
    : IQueryHandler<GetArticleReceiptByIdQuery, GetArticleReceiptByIdQueryResult>
{
    public async Task<Result<GetArticleReceiptByIdQueryResult>> HandleAsync(
        GetArticleReceiptByIdQuery query, CancellationToken cancellationToken)
    {
        var receipt = await repository.GetBySpecificationAsync(
            new ArticleReceiptByIdSpecification(query.Id), cancellationToken);

        if (receipt is null)
            return Result.Fail<GetArticleReceiptByIdQueryResult>(
                "Recepción no encontrada.", "articlereceipt.notfound");

        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(query.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<GetArticleReceiptByIdQueryResult>(
                "El usuario autenticado no existe.", "articlereceipt.user.notfound");

        if (actor.Role.Name == RoleNames.UsuarioEmpresa
            && receipt.Article.SupplierId != actor.SupplierId)
            return Result.Fail<GetArticleReceiptByIdQueryResult>(
                "La recepción no pertenece al proveedor del usuario.", "articlereceipt.access.forbidden");

        return Result.Success(new GetArticleReceiptByIdQueryResult
        {
            Id = receipt.Id,
            ArticleId = receipt.ArticleId,
            ArticleSku = receipt.Article.Sku,
            ArticleName = receipt.Article.Name,
            Count = receipt.Count,
            CreatedAt = receipt.CreatedAt
        });
    }
}
