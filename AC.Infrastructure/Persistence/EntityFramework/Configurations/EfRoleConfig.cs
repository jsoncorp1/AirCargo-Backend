using AC.Domain.Modules.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfRoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name)     // ajustá a las props reales de Role
            .IsRequired()
            .HasMaxLength(100);

        // La relación 1-N con User se arma sola por convención
        // (User.RoleId + User.Role + Role.Users); el delete queda en
        // Restrict por el loop global del CoreDbContext.
    }
}