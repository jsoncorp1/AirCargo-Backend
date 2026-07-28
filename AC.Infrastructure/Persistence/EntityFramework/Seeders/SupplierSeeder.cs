using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AC.Infrastructure.Persistence.EntityFramework.Seeders;

public class SupplierSeeder(CoreDbContext dbContext, ILogger<SupplierSeeder> logger)
{
    public static readonly Guid LaminasId   = new("a1111111-1111-1111-1111-111111111111");
    public static readonly Guid ViralshopId = new("a2222222-2222-2222-2222-222222222222");

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existing = (await dbContext.Suppliers
            .Select(s => s.Name)
            .ToListAsync(cancellationToken)).ToHashSet();

        var now = DateTime.UtcNow;
        Supplier Def(Guid id, string name, string description, BolivianDepartment department) => new()
        {
            Id = id,
            Name = name,
            Description = description,
            Department = department,
            ArticleQuantity = 0,
            UserQuantity = 0,
            Active = true,
            CreatedAt = now,
            CreatedBy = "seeder",
        };

        var definitions = new[]
            {
                Def(LaminasId,   "Laminas",   "Proveedor de láminas",       BolivianDepartment.SantaCruz),
                Def(ViralshopId, "Viralshop", "Tienda de productos virales", BolivianDepartment.SantaCruz),
            }
            .Where(s => !existing.Contains(s.Name))
            .ToArray();

        if (definitions.Length == 0)
        {
            logger.LogInformation("Proveedores ya están seedeados.");
            return;
        }

        dbContext.Suppliers.AddRange(definitions);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} proveedores.", definitions.Length);
    }
}
