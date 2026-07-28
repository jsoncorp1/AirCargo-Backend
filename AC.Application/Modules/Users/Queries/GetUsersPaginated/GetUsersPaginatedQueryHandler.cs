using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Roles;
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

        var actor = await repository.GetBySpecificationAsync(
            new UserByIdSpecification(query.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<GetUsersPaginatedQueryResult>(
                "El usuario autenticado no existe.", "user.actor.notfound");

        // El admin solo lista conductores de su sucursal; se ignoran los filtros del request.
        var roleId = query.RoleId;
        var supplierId = query.SupplierId;
        string? roleName = null;
        Guid? branchOfficeId = null;

        if (actor.Role.Name == RoleNames.Admin)
        {
            if (actor.BranchOfficeId is null)
                return Result.Fail<GetUsersPaginatedQueryResult>(
                    "El usuario no tiene una sucursal asignada.", "user.actor.nobranch");

            roleId = null;
            supplierId = null;
            roleName = RoleNames.Conductor;
            branchOfficeId = actor.BranchOfficeId;
        }

        var spec = new UserPaginationSpecification(
            page, perPage, roleId, supplierId, roleName, branchOfficeId);
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
                SupplierName = u.Supplier != null ? u.Supplier.Name : null,
                BranchOfficeId = u.BranchOfficeId,
                BranchOfficeCode = u.BranchOffice != null ? u.BranchOffice.Code : null,
                BranchOfficeCity = u.BranchOffice != null ? u.BranchOffice.City : null
            })
        });
    }
}