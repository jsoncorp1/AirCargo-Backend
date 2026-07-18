using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommand : ICommand<CreateSupplierCommandResult>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
