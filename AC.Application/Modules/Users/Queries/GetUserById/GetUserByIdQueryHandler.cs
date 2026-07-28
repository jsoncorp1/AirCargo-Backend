using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(
    IRepository<User> repository)
    : IQueryHandler<GetUserByIdQuery, GetUserByIdQueryResult>
{
    public async Task<Result<GetUserByIdQueryResult>> HandleAsync(
        GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await repository.GetBySpecificationAsync(
            new UserByIdSpecification(query.Id), cancellationToken);

        if (user is null)
            return Result.Fail<GetUserByIdQueryResult>(
                "El usuario no existe.", "user.notfound");

        var actor = await repository.GetBySpecificationAsync(
            new UserByIdSpecification(query.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<GetUserByIdQueryResult>(
                "El usuario autenticado no existe.", "user.actor.notfound");

        // El admin solo ve conductores de su sucursal.
        if (actor.Role.Name == RoleNames.Admin
            && (user.Role.Name != RoleNames.Conductor || user.BranchOfficeId != actor.BranchOfficeId))
            return Result.Fail<GetUserByIdQueryResult>(
                "Un admin solo puede ver conductores de su sucursal.", "user.access.forbidden");

        return Result.Success(new GetUserByIdQueryResult
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Dni = user.Dni,
            RoleId = user.RoleId,
            RoleName = user.Role.Name,
            SupplierId = user.SupplierId,
            SupplierName = user.Supplier?.Name,
            BranchOfficeId = user.BranchOfficeId,
            BranchOfficeCode = user.BranchOffice?.Code,
            BranchOfficeCity = user.BranchOffice?.City
        });
    }
}