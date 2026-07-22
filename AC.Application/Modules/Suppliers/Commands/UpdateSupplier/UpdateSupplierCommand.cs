using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommand : ICommand<UpdateSupplierCommandResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BolivianDepartment Department { get; set; }
}
