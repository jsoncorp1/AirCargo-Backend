using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Articles.Specifications;
using AC.Application.Modules.OrderDeliveries.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.OrderDeliveries.Commands.UpdateOrderDelivery;

public class UpdateOrderDeliveryCommandHandler(
    IRepository<OrderDelivery> orderDeliveryRepository,
    IRepository<OrderDeliveryDetail> orderDeliveryDetailRepository,
    IRepository<Article> articleRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateOrderDeliveryCommand, UpdateOrderDeliveryCommandResult>
{
    public async Task<Result<UpdateOrderDeliveryCommandResult>> HandleAsync(
        UpdateOrderDeliveryCommand command, CancellationToken cancellationToken)
    {
        var order = await orderDeliveryRepository.GetBySpecificationAsync(
            new OrderDeliveryByIdSpecification(command.Id), cancellationToken);

        if (order is null)
            return Result.Fail<UpdateOrderDeliveryCommandResult>(
                "Orden no encontrada.", "orderdelivery.notfound");

        if (order.Shipment is { Active: true })
            return Result.Fail<UpdateOrderDeliveryCommandResult>(
                "La orden ya fue atendida y no se puede editar.", "orderdelivery.alreadyattended");

        if (command.Lines.Count == 0)
            return Result.Fail<UpdateOrderDeliveryCommandResult>(
                "La orden debe tener al menos una línea.", "orderdelivery.lines.required");

        var articleCache = new Dictionary<Guid, Article>();

        async Task<Result<Article>> GetOrCacheArticleAsync(Guid articleId)
        {
            if (articleCache.TryGetValue(articleId, out var cached))
                return Result.Success(cached);

            var article = await articleRepository.GetBySpecificationAsync(
                new ArticleByIdSpecification(articleId), cancellationToken);

            if (article is null)
                return Result.Fail<Article>(
                    "Uno de los artículos indicados no existe.", "orderdelivery.article.notfound");

            articleCache[articleId] = article;
            return Result.Success(article);
        }

        var oldDetails = order.OrderDeliveryDetails.ToList();

        foreach (var oldDetail in oldDetails)
        {
            var articleResult = await GetOrCacheArticleAsync(oldDetail.ArticleId);
            if (articleResult.Failure)
                return Result.Fail<UpdateOrderDeliveryCommandResult>(
                    articleResult.Error, articleResult.ErrorKey);

            articleResult.Value.Count += oldDetail.Quantity;
            oldDetail.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy
        }

        var newDetails = new List<OrderDeliveryDetail>();
        decimal totalPrice = 0;

        foreach (var line in command.Lines)
        {
            if (line.Quantity <= 0)
                return Result.Fail<UpdateOrderDeliveryCommandResult>(
                    "La cantidad de cada línea debe ser mayor a cero.", "orderdelivery.quantity.invalid");

            var articleResult = await GetOrCacheArticleAsync(line.ArticleId);
            if (articleResult.Failure)
                return Result.Fail<UpdateOrderDeliveryCommandResult>(
                    articleResult.Error, articleResult.ErrorKey);

            var article = articleResult.Value;

            if (article.SupplierId != order.SupplierId)
                return Result.Fail<UpdateOrderDeliveryCommandResult>(
                    "El artículo no pertenece al proveedor de la orden.", "orderdelivery.article.invalidsupplier");

            if (article.Count < line.Quantity)
                return Result.Fail<UpdateOrderDeliveryCommandResult>(
                    $"Stock insuficiente para el artículo '{article.Name}'.", "orderdelivery.stock.insufficient");

            article.Count -= line.Quantity;

            decimal lineTotal = line.Quantity * line.UnitPrice;
            totalPrice += lineTotal;

            newDetails.Add(new OrderDeliveryDetail
            {
                Id = Guid.NewGuid(),
                OrderDeliveryId = order.Id,
                ArticleId = article.Id,
                ArticleName = article.Name,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LineTotal = lineTotal
            });
        }

        order.Departamento = command.Departamento;
        order.ClientePhone = command.ClientePhone;
        order.ClienteNombreCompleto = command.ClienteNombreCompleto;
        order.ClienteDireccion = command.ClienteDireccion;
        order.TipoEntrega = command.TipoEntrega;
        order.TotalPrice = totalPrice;

        await orderDeliveryRepository.UpdateAsync(order, cancellationToken);

        if (oldDetails.Count > 0)
            await orderDeliveryDetailRepository.UpdateAsync(oldDetails.ToArray(), cancellationToken);

        await orderDeliveryDetailRepository.SaveAsync(newDetails.ToArray(), cancellationToken);
        await articleRepository.UpdateAsync(articleCache.Values.ToArray(), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateOrderDeliveryCommandResult
        {
            Id = order.Id,
            SupplierId = order.SupplierId,
            UserId = order.UserId,
            Departamento = order.Departamento,
            ClientePhone = order.ClientePhone,
            ClienteNombreCompleto = order.ClienteNombreCompleto,
            ClienteDireccion = order.ClienteDireccion,
            TipoEntrega = order.TipoEntrega,
            TotalPrice = order.TotalPrice,
            Details = newDetails.Select(d => new OrderDeliveryDetailResult
            {
                Id = d.Id,
                ArticleId = d.ArticleId,
                ArticleName = d.ArticleName,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                LineTotal = d.LineTotal
            }).ToList()
        });
    }
}
