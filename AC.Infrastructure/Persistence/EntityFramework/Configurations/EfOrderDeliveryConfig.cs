using AC.Domain.Modules.OrderDeliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfOrderDeliveryConfig : IEntityTypeConfiguration<OrderDelivery>
{
    public void Configure(EntityTypeBuilder<OrderDelivery> builder)
    {
        builder.Property(o => o.OrderType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.OriginDepartment)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.DestinationDepartment)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.DeliveryType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.SenderFullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.SenderPhone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(o => o.SenderAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(o => o.ClientPhone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(o => o.ClientFullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.ClientAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(o => o.TotalPrice)
            .HasPrecision(18, 2);

        // La relación 1-1 con Shipment se configura desde EfShipmentConfig
        // (Shipment.OrderDeliveryId es la FK); el delete queda en Restrict
        // por el loop global del CoreDbContext.
    }
}
