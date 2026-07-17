using AC.Domain.Modules.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AC.Infrastructure.Persistence.EntityFramework.Seeders;

public class RoleSeeder(CoreDbContext dbContext, ILogger<RoleSeeder> logger)
{
    public static readonly Guid SuperAdminId     = new("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AdminId          = new("22222222-2222-2222-2222-222222222222");
    public static readonly Guid UsuarioEmpresaId = new("33333333-3333-3333-3333-333333333333");

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existing = (await dbContext.Roles
            .Select(r => r.Name)
            .ToListAsync(cancellationToken)).ToHashSet();

        var now = DateTime.UtcNow;
        Role Def(Guid id, string name, string description) => new()
        {
            Id = id,
            Name = name,
            Description = description,
            Active = true,
            CreatedAt = now,
            CreatedBy = "seeder",
        };

        var definitions = new[]
            {
                Def(SuperAdminId,     "superadmin",     "Acceso total al sistema"),
                Def(AdminId,          "admin",          "Administración general"),
                Def(UsuarioEmpresaId, "usuarioempresa", "Usuario de empresa proveedora"),
            }
            .Where(r => !existing.Contains(r.Name))
            .ToArray();

        if (definitions.Length == 0)
        {
            logger.LogInformation("Roles ya están seedeados.");
            return;
        }

        dbContext.Roles.AddRange(definitions);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} roles.", definitions.Length);
    }
}