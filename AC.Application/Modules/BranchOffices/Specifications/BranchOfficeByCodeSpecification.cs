using AC.Domain.Modules.BranchOffices;
using Ardalis.Specification;

namespace AC.Application.Modules.BranchOffices.Specifications;

public class BranchOfficeByCodeSpecification : Specification<BranchOffice>
{
    // Solo compite contra sucursales activas: una sucursal soft-deleteada
    // no debe seguir "ocupando" su código (mismo criterio que el índice
    // único parcial en EfBranchOfficeConfig).
    public BranchOfficeByCodeSpecification(string code) =>
        Query.Where(b => b.Code == code && b.Active);
}
