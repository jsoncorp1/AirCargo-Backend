using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryHandler(IRepository<User> repository)
    : IQueryHandler<GetUsersPaginatedQuery, GetUsersPaginatedQueryResult>
{
    public async Task<Result<GetUsersPaginatedQueryResult>> HandleAsync(
        GetUsersPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new UserPaginationSpecification(page, perPage, query.RoleId, query.SupplierId);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetUsersPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(u => new UserPaginatedItem
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Dni = u.Dni,
                RoleId = u.RoleId,
                RoleName = u.Role.Name,
                SupplierId = u.SupplierId,
                SupplierName = u.Supplier != null ? u.Supplier.Name : null
            })
        });
    }
}