using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommand : ICommand<DeleteSupplierCommandResult>
{
    public Guid Id { get; set; }
}
