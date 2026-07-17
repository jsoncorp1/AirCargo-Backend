using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Users.Specifications;
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

        return Result.Success(new GetUserByIdQueryResult
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Dni = user.Dni,
            RoleId = user.RoleId,
            RoleName = user.Role.Name
        });
    }
}