using AC.Domain.Modules.BranchOffices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfBranchOfficeConfig : IEntityTypeConfiguration<BranchOffice>
{
    public void Configure(EntityTypeBuilder<BranchOffice> builder)
    {
        builder.Property(b => b.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.BolivianDepartment)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Address)
            .HasMaxLength(300);

        builder.Property(b => b.Phone)
            .IsRequired()
            .HasMaxLength(30);

        // Código único solo entre sucursales activas: una sucursal
        // soft-deleteada no debe bloquear la reutilización de su código.
        builder.HasIndex(b => b.Code)
            .IsUnique()
            .HasFilter("active = true");
    }
}
