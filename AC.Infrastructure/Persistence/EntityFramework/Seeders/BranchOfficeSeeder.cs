using AC.Domain.Modules.BranchOffices;
using AC.Domain.Modules.OrderDeliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AC.Infrastructure.Persistence.EntityFramework.Seeders;

public class BranchOfficeSeeder(CoreDbContext dbContext, ILogger<BranchOfficeSeeder> logger)
{
    public static readonly Guid SantaCruzId    = new("b1111111-1111-1111-1111-111111111111");
    public static readonly Guid LaPazId        = new("b2222222-2222-2222-2222-222222222222");
    public static readonly Guid ElAltoId       = new("b3333333-3333-3333-3333-333333333333");
    public static readonly Guid TrinidadId     = new("b4444444-4444-4444-4444-444444444444");
    public static readonly Guid GuayaramerinId = new("b5555555-5555-5555-5555-555555555555");

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existing = (await dbContext.BranchOffices
            .Select(b => b.Code)
            .ToListAsync(cancellationToken)).ToHashSet();

        var now = DateTime.UtcNow;
        BranchOffice Def(Guid id, string code, string city, BolivianDepartment department, string phone) => new()
        {
            Id = id,
            Code = code,
            City = city,
            BolivianDepartment = department,
            Phone = phone,
            Active = true,
            CreatedAt = now,
            CreatedBy = "seeder",
        };

        var definitions = new[]
            {
                Def(SantaCruzId,    "SCZ-01", "Santa Cruz",    BolivianDepartment.SantaCruz, "70000001"),
                Def(LaPazId,        "LPZ-01", "La Paz",        BolivianDepartment.LaPaz,     "70000002"),
                Def(ElAltoId,       "EAL-01", "El Alto",       BolivianDepartment.LaPaz,     "70000003"),
                Def(TrinidadId,     "TDD-01", "Trinidad",      BolivianDepartment.Beni,      "70000004"),
                Def(GuayaramerinId, "GYA-01", "Guayaramerín",  BolivianDepartment.Beni,      "70000005"),
            }
            .Where(b => !existing.Contains(b.Code))
            .ToArray();

        if (definitions.Length == 0)
        {
            logger.LogInformation("Sucursales ya están seedeadas.");
            return;
        }

        dbContext.BranchOffices.AddRange(definitions);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} sucursales.", definitions.Length);
    }
}
