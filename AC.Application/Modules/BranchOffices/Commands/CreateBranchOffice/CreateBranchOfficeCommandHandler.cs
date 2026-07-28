using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.BranchOffices.Specifications;
using AC.Domain.Modules.BranchOffices;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.BranchOffices.Commands.CreateBranchOffice;

public class CreateBranchOfficeCommandHandler(
    IRepository<BranchOffice> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateBranchOfficeCommand, CreateBranchOfficeCommandResult>
{
    public async Task<Result<CreateBranchOfficeCommandResult>> HandleAsync(
        CreateBranchOfficeCommand command, CancellationToken cancellationToken)
    {
        Result validation = await ValidateAsync(command, cancellationToken);
        if (validation.Failure)
            return Result.Fail<CreateBranchOfficeCommandResult>(validation.Error, validation.ErrorKey);

        var branchOffice = new BranchOffice
        {
            Id = Guid.NewGuid(),
            Code = command.Code.Trim(),
            BolivianDepartment = command.BolivianDepartment,
            City = command.City.Trim(),
            Address = command.Address?.Trim(),
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Phone = command.Phone.Trim()
            // Active / CreatedAt / CreatedBy los pone el AuditInterceptor
        };

        await repository.SaveAsync(branchOffice, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateBranchOfficeCommandResult
        {
            Id = branchOffice.Id,
            Code = branchOffice.Code
        });
    }

    private async Task<Result> ValidateAsync(
        CreateBranchOfficeCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Result.Fail("El código es obligatorio.", "branchoffice.code.required");

        if (string.IsNullOrWhiteSpace(command.City))
            return Result.Fail("La ciudad es obligatoria.", "branchoffice.city.required");

        if (string.IsNullOrWhiteSpace(command.Phone))
            return Result.Fail("El teléfono es obligatorio.", "branchoffice.phone.required");

        var existing = await repository.GetBySpecificationAsync(
            new BranchOfficeByCodeSpecification(command.Code.Trim()), cancellationToken);

        if (existing is not null)
            return Result.Fail("Ya existe una sucursal con ese código.", "branchoffice.code.duplicate");

        return Result.Success();
    }
}
