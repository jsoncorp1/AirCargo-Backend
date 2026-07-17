using AC.Domain.Modules.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfUserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FullName)  // ajustá a las props reales de User
            .IsRequired()
            .HasMaxLength(100);
    }
}