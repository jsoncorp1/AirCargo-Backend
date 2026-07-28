using AC.Domain.Modules.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AC.Infrastructure.Persistence.EntityFramework.Seeders;

public class UserSeeder(CoreDbContext dbContext, ILogger<UserSeeder> logger)
{
    // Contraseña de todos los usuarios semilla; el hash se genera al sembrar.
    private const string SeedPassword = "Harold123";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existing = (await dbContext.Users
            .Select(u => u.Email)
            .ToListAsync(cancellationToken)).ToHashSet();

        var now = DateTime.UtcNow;
        var seedPasswordHash = BCrypt.Net.BCrypt.HashPassword(SeedPassword);
        User Def(Guid id, string fullName, string email, Guid roleId,
            Guid? supplierId = null, Guid? branchOfficeId = null) => new()
        {
            Id = id,
            FullName = fullName,
            Email = email,
            PasswordHash = seedPasswordHash,
            PhoneNumber = string.Empty,
            Dni = string.Empty,
            RoleId = roleId,
            SupplierId = supplierId,
            BranchOfficeId = branchOfficeId,
            Active = true,
            CreatedAt = now,
            CreatedBy = "seeder",
        };

        var definitions = new[]
            {
                Def(new Guid("c1111111-1111-1111-1111-111111111111"), "Harold",   "harold@gmail.com",   RoleSeeder.SuperAdminId,     branchOfficeId: BranchOfficeSeeder.SantaCruzId),
                Def(new Guid("c2222222-2222-2222-2222-222222222222"), "Damian",   "damian@gmail.com",   RoleSeeder.UsuarioEmpresaId, supplierId: SupplierSeeder.LaminasId),
                Def(new Guid("c3333333-3333-3333-3333-333333333333"), "Ruben",    "ruben@gmail.com",    RoleSeeder.UsuarioEmpresaId, supplierId: SupplierSeeder.ViralshopId),
                Def(new Guid("c4444444-4444-4444-4444-444444444444"), "Camila",   "camila@gmail.com",   RoleSeeder.AdminId,          branchOfficeId: BranchOfficeSeeder.SantaCruzId),
                Def(new Guid("c5555555-5555-5555-5555-555555555555"), "Camilo",   "camilo@gmail.com",   RoleSeeder.AdminId,          branchOfficeId: BranchOfficeSeeder.LaPazId),
                Def(new Guid("c6666666-6666-6666-6666-666666666666"), "Wilson",   "wilson@gmail.com",   RoleSeeder.ConductorId,      branchOfficeId: BranchOfficeSeeder.SantaCruzId),
                Def(new Guid("c7777777-7777-7777-7777-777777777777"), "Rolando",  "rolando@gmail.com",  RoleSeeder.ConductorId,      branchOfficeId: BranchOfficeSeeder.LaPazId),
                Def(new Guid("c8888888-8888-8888-8888-888888888888"), "Jhonatan", "jhonatan@gmail.com", RoleSeeder.ConductorId,      branchOfficeId: BranchOfficeSeeder.LaPazId),
            }
            .Where(u => !existing.Contains(u.Email))
            .ToArray();

        if (definitions.Length == 0)
        {
            logger.LogInformation("Usuarios ya están seedeados.");
            return;
        }

        // Mantiene el contador desnormalizado igual que CreateUserCommandHandler.
        foreach (var group in definitions
                     .Where(u => u.SupplierId is not null)
                     .GroupBy(u => u.SupplierId!.Value))
        {
            var supplier = await dbContext.Suppliers
                .FirstAsync(s => s.Id == group.Key, cancellationToken);
            supplier.UserQuantity += group.Count();
        }

        dbContext.Users.AddRange(definitions);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} usuarios.", definitions.Length);
    }
}
