using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Articles.Specifications;
using AC.Application.Modules.OrderDeliveries.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.OrderDeliveries.Commands.DeleteOrderDelivery;

public class DeleteOrderDeliveryCommandHandler(
    IRepository<OrderDelivery> orderDeliveryRepository,
    IRepository<OrderDeliveryDetail> orderDeliveryDetailRepository,
    IRepository<Article> articleRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteOrderDeliveryCommand, DeleteOrderDeliveryCommandResult>
{
    public async Task<Result<DeleteOrderDeliveryCommandResult>> HandleAsync(
        DeleteOrderDeliveryCommand command, CancellationToken cancellationToken)
    {
        var order = await orderDeliveryRepository.GetBySpecificationAsync(
            new OrderDeliveryByIdSpecification(command.Id), cancellationToken);

        if (order is null)
            return Result.Fail<DeleteOrderDeliveryCommandResult>(
                "Orden no encontrada.", "orderdelivery.notfound");

        if (order.Shipment is { Active: true })
            return Result.Fail<DeleteOrderDeliveryCommandResult>(
                "La orden ya fue atendida y no se puede cancelar.", "orderdelivery.alreadyattended");

        var articleCache = new Dictionary<Guid, Article>();

        foreach (var detail in order.OrderDeliveryDetails)
        {
            if (!articleCache.TryGetValue(detail.ArticleId, out var article))
            {
                article = await articleRepository.GetBySpecificationAsync(
                    new ArticleByIdSpecification(detail.ArticleId), cancellationToken);

                if (article is null)
                    return Result.Fail<DeleteOrderDeliveryCommandResult>(
                        "Uno de los artículos de la orden ya no existe.", "orderdelivery.article.notfound");

                articleCache[detail.ArticleId] = article;
            }

            article.Count += detail.Quantity;
            detail.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy
        }

        order.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy

        await orderDeliveryRepository.UpdateAsync(order, cancellationToken);

        if (order.OrderDeliveryDetails.Count > 0)
            await orderDeliveryDetailRepository.UpdateAsync(order.OrderDeliveryDetails.ToArray(), cancellationToken);

        if (articleCache.Count > 0)
            await articleRepository.UpdateAsync(articleCache.Values.ToArray(), cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteOrderDeliveryCommandResult { Id = order.Id });
    }
}
