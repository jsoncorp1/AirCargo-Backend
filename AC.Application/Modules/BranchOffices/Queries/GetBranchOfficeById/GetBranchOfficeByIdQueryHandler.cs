using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.BranchOffices.Specifications;
using AC.Domain.Modules.BranchOffices;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.BranchOffices.Queries.GetBranchOfficeById;

public class GetBranchOfficeByIdQueryHandler(IRepository<BranchOffice> repository)
    : IQueryHandler<GetBranchOfficeByIdQuery, GetBranchOfficeByIdQueryResult>
{
    public async Task<Result<GetBranchOfficeByIdQueryResult>> HandleAsync(
        GetBranchOfficeByIdQuery query, CancellationToken cancellationToken)
    {
        var branchOffice = await repository.GetBySpecificationAsync(
            new BranchOfficeByIdSpecification(query.Id), cancellationToken);

        if (branchOffice is null)
            return Result.Fail<GetBranchOfficeByIdQueryResult>(
                "Sucursal no encontrada.", "branchoffice.notfound");

        return Result.Success(new GetBranchOfficeByIdQueryResult
        {
            Id = branchOffice.Id,
            Code = branchOffice.Code,
            BolivianDepartment = branchOffice.BolivianDepartment,
            City = branchOffice.City,
            Address = branchOffice.Address,
            Latitude = branchOffice.Latitude,
            Longitude = branchOffice.Longitude,
            Phone = branchOffice.Phone,
            Active = branchOffice.Active
        });
    }
}
