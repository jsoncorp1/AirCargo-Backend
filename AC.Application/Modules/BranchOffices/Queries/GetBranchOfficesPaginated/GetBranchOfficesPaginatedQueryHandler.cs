using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.BranchOffices.Specifications;
using AC.Domain.Modules.BranchOffices;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.BranchOffices.Queries.GetBranchOfficesPaginated;

public class GetBranchOfficesPaginatedQueryHandler(IRepository<BranchOffice> repository)
    : IQueryHandler<GetBranchOfficesPaginatedQuery, GetBranchOfficesPaginatedQueryResult>
{
    public async Task<Result<GetBranchOfficesPaginatedQueryResult>> HandleAsync(
        GetBranchOfficesPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new BranchOfficePaginationSpecification(page, perPage);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetBranchOfficesPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(b => new BranchOfficePaginatedItem
            {
                Id = b.Id,
                Code = b.Code,
                BolivianDepartment = b.BolivianDepartment,
                City = b.City,
                Address = b.Address,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                Phone = b.Phone,
                Active = b.Active
            })
        });
    }
}
