using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Articles.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.OrderDeliveries.Commands.CreateOrderDelivery;

public class CreateOrderDeliveryCommandHandler(
    IRepository<OrderDelivery> orderDeliveryRepository,
    IRepository<OrderDeliveryDetail> orderDeliveryDetailRepository,
    IRepository<User> userRepository,
    IRepository<Article> articleRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateOrderDeliveryCommand, CreateOrderDeliveryCommandResult>
{
    public async Task<Result<CreateOrderDeliveryCommandResult>> HandleAsync(
        CreateOrderDeliveryCommand command, CancellationToken cancellationToken)
    {
        if (command.Lines.Count == 0)
            return Result.Fail<CreateOrderDeliveryCommandResult>(
                "La orden debe tener al menos una línea.", "orderdelivery.lines.required");

        var user = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(command.UserId), cancellationToken);

        if (user is null)
            return Result.Fail<CreateOrderDeliveryCommandResult>(
                "El usuario indicado no existe.", "orderdelivery.user.notfound");

        if (user.SupplierId is null)
            return Result.Fail<CreateOrderDeliveryCommandResult>(
                "El usuario no pertenece a ningún proveedor.", "orderdelivery.user.notsupplier");

        var supplierId = user.SupplierId.Value;
        var articleCache = new Dictionary<Guid, Article>();
        var details = new List<OrderDeliveryDetail>();
        decimal totalPrice = 0;

        foreach (var line in command.Lines)
        {
            if (line.Quantity <= 0)
                return Result.Fail<CreateOrderDeliveryCommandResult>(
                    "La cantidad de cada línea debe ser mayor a cero.", "orderdelivery.quantity.invalid");

            if (!articleCache.TryGetValue(line.ArticleId, out var article))
            {
                article = await articleRepository.GetBySpecificationAsync(
                    new ArticleByIdSpecification(line.ArticleId), cancellationToken);

                if (article is null)
                    return Result.Fail<CreateOrderDeliveryCommandResult>(
                        "Uno de los artículos indicados no existe.", "orderdelivery.article.notfound");

                if (article.SupplierId != supplierId)
                    return Result.Fail<CreateOrderDeliveryCommandResult>(
                        "El artículo no pertenece al proveedor del usuario.", "orderdelivery.article.invalidsupplier");

                articleCache[line.ArticleId] = article;
            }

            if (article.Count < line.Quantity)
                return Result.Fail<CreateOrderDeliveryCommandResult>(
                    $"Stock insuficiente para el artículo '{article.Name}'.", "orderdelivery.stock.insufficient");

            article.Count -= line.Quantity;

            decimal lineTotal = line.Quantity * line.UnitPrice;
            totalPrice += lineTotal;

            details.Add(new OrderDeliveryDetail
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                ArticleName = article.Name,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LineTotal = lineTotal
            });
        }

        var orderDelivery = new OrderDelivery
        {
            Id = Guid.NewGuid(),
            SupplierId = supplierId,
            UserId = command.UserId,
            Departamento = command.Departamento,
            ClientePhone = command.ClientePhone,
            ClienteNombreCompleto = command.ClienteNombreCompleto,
            ClienteDireccion = command.ClienteDireccion,
            TipoEntrega = command.TipoEntrega,
            TotalPrice = totalPrice
        };

        foreach (var detail in details)
            detail.OrderDeliveryId = orderDelivery.Id;

        await orderDeliveryRepository.SaveAsync(orderDelivery, cancellationToken);
        await orderDeliveryDetailRepository.SaveAsync(details.ToArray(), cancellationToken);
        await articleRepository.UpdateAsync(articleCache.Values.ToArray(), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateOrderDeliveryCommandResult
        {
            Id = orderDelivery.Id,
            SupplierId = orderDelivery.SupplierId,
            UserId = orderDelivery.UserId,
            Departamento = orderDelivery.Departamento,
            ClientePhone = orderDelivery.ClientePhone,
            ClienteNombreCompleto = orderDelivery.ClienteNombreCompleto,
            ClienteDireccion = orderDelivery.ClienteDireccion,
            TipoEntrega = orderDelivery.TipoEntrega,
            TotalPrice = orderDelivery.TotalPrice,
            Details = details.Select(d => new OrderDeliveryDetailResult
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
