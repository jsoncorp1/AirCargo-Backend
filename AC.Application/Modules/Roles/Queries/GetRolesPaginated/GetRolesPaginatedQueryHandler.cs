using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Roles.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Roles.Queries.GetRolesPaginated;

public class GetRolesPaginatedQueryHandler(IRepository<Role> repository)
    : IQueryHandler<GetRolesPaginatedQuery, GetRolesPaginatedQueryResult>
{
    public async Task<Result<GetRolesPaginatedQueryResult>> HandleAsync(
        GetRolesPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new RolePaginationSpecification(page, perPage);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetRolesPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(r => new RolePaginatedItem
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            })
        });
    }
}