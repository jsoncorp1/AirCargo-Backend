using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    IRepository<User> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteUserCommand, DeleteUserCommandResult>
{
    public async Task<Result<DeleteUserCommandResult>> HandleAsync(
        DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await repository.GetBySpecificationAsync(
            new UserByIdSpecification(command.Id), cancellationToken);

        if (user is null)
            return Result.Fail<DeleteUserCommandResult>("Usuario no encontrado.", "user.notfound");

        user.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy

        await repository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteUserCommandResult { Id = user.Id });
    }
}