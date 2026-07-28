using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.BranchOffices.Specifications;
using AC.Domain.Modules.BranchOffices;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.BranchOffices.Commands.DeleteBranchOffice;

public class DeleteBranchOfficeCommandHandler(
    IRepository<BranchOffice> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteBranchOfficeCommand, DeleteBranchOfficeCommandResult>
{
    public async Task<Result<DeleteBranchOfficeCommandResult>> HandleAsync(
        DeleteBranchOfficeCommand command, CancellationToken cancellationToken)
    {
        var branchOffice = await repository.GetBySpecificationAsync(
            new BranchOfficeByIdSpecification(command.Id), cancellationToken);

        if (branchOffice is null)
            return Result.Fail<DeleteBranchOfficeCommandResult>(
                "Sucursal no encontrada.", "branchoffice.notfound");

        branchOffice.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy

        await repository.UpdateAsync(branchOffice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteBranchOfficeCommandResult { Id = branchOffice.Id });
    }
}
