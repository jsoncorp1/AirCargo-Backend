using AC.Domain.Modules.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AC.Infrastructure.Persistence.EntityFramework.Seeders;

public class UserSeeder(CoreDbContext dbContext, ILogger<UserSeeder> logger)
{
    private const string DefaultPassword = "password";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existing = (await dbContext.Users
            .Select(u => u.Email)
            .ToListAsync(cancellationToken)).ToHashSet();

        var now = DateTime.UtcNow;
        User Def(string fullName, string email, Guid roleId) => new()
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword),
            PhoneNumber = string.Empty,
            Dni = string.Empty,
            RoleId = roleId,
            Active = true,
            CreatedAt = now,
            CreatedBy = "seeder",
        };

        var definitions = new[]
            {
                Def("Super Admin",     "superadmin@superadmin.com",         RoleSeeder.SuperAdminId),
                Def("Admin",           "admin@admin.com",                   RoleSeeder.AdminId),
                Def("Usuario Empresa", "usuarioempresa@usuarioempresa.com", RoleSeeder.UsuarioEmpresaId),
            }
            .Where(u => !existing.Contains(u.Email))
            .ToArray();

        if (definitions.Length == 0)
        {
            logger.LogInformation("Usuarios ya están seedeados.");
            return;
        }

        dbContext.Users.AddRange(definitions);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} usuarios.", definitions.Length);
    }
}