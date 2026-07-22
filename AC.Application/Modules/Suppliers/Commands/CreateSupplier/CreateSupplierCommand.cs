using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommand : ICommand<CreateSupplierCommandResult>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BolivianDepartment Department { get; set; }
}
