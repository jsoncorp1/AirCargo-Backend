using AC.Domain.Modules.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfSupplierConfig : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Department)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // La relación 1-N con User se arma sola por convención
        // (User.SupplierId + User.Supplier + Supplier.Users); el delete queda en
        // Restrict por el loop global del CoreDbContext.
    }
}
