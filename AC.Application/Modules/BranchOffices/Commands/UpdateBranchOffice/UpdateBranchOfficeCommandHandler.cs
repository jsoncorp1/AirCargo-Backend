using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.BranchOffices.Specifications;
using AC.Domain.Modules.BranchOffices;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.BranchOffices.Commands.UpdateBranchOffice;

public class UpdateBranchOfficeCommandHandler(
    IRepository<BranchOffice> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateBranchOfficeCommand, UpdateBranchOfficeCommandResult>
{
    public async Task<Result<UpdateBranchOfficeCommandResult>> HandleAsync(
        UpdateBranchOfficeCommand command, CancellationToken cancellationToken)
    {
        var branchOffice = await repository.GetBySpecificationAsync(
            new BranchOfficeByIdSpecification(command.Id), cancellationToken);

        if (branchOffice is null)
            return Result.Fail<UpdateBranchOfficeCommandResult>(
                "Sucursal no encontrada.", "branchoffice.notfound");

        Result validation = await ValidateAsync(command, branchOffice, cancellationToken);
        if (validation.Failure)
            return Result.Fail<UpdateBranchOfficeCommandResult>(validation.Error, validation.ErrorKey);

        branchOffice.Code = command.Code.Trim();
        branchOffice.BolivianDepartment = command.BolivianDepartment;
        branchOffice.City = command.City.Trim();
        branchOffice.Address = command.Address?.Trim();
        branchOffice.Latitude = command.Latitude;
        branchOffice.Longitude = command.Longitude;
        branchOffice.Phone = command.Phone.Trim();

        await repository.UpdateAsync(branchOffice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateBranchOfficeCommandResult
        {
            Id = branchOffice.Id,
            Code = branchOffice.Code
        });
    }

    private async Task<Result> ValidateAsync(
        UpdateBranchOfficeCommand command, BranchOffice branchOffice,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Result.Fail("El código es obligatorio.", "branchoffice.code.required");

        if (string.IsNullOrWhiteSpace(command.City))
            return Result.Fail("La ciudad es obligatoria.", "branchoffice.city.required");

        if (string.IsNullOrWhiteSpace(command.Phone))
            return Result.Fail("El teléfono es obligatorio.", "branchoffice.phone.required");

        var duplicate = await repository.GetBySpecificationAsync(
            new BranchOfficeByCodeSpecification(command.Code.Trim()), cancellationToken);

        if (duplicate is not null && duplicate.Id != branchOffice.Id)
            return Result.Fail("Ya existe otra sucursal con ese código.", "branchoffice.code.duplicate");

        return Result.Success();
    }
}
