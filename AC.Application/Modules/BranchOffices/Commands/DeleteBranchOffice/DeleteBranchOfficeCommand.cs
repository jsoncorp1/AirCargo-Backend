using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.BranchOffices.Commands.DeleteBranchOffice;

public class DeleteBranchOfficeCommand : ICommand<DeleteBranchOfficeCommandResult>
{
    public Guid Id { get; set; }
}
