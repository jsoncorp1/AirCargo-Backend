using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Suppliers.Queries.GetSuppliersPaginated;

public class GetSuppliersPaginatedQueryHandler(IRepository<Supplier> repository)
    : IQueryHandler<GetSuppliersPaginatedQuery, GetSuppliersPaginatedQueryResult>
{
    public async Task<Result<GetSuppliersPaginatedQueryResult>> HandleAsync(
        GetSuppliersPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new SupplierPaginationSpecification(page, perPage, query.Department);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetSuppliersPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(s => new SupplierPaginatedItem
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Department = s.Department.ToString(),
                ArticleQuantity = s.ArticleQuantity,
                UserQuantity = s.UserQuantity
            })
        });
    }
}
