using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfShipmentConfig : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.Property(s => s.TotalWeight)
            .HasPrecision(18, 2);

        builder.Property(s => s.ShippingPrice)
            .HasPrecision(18, 2);

        // Correlativo único a nivel de tabla (incluye filas soft-deleted) para
        // que el numero de guia autogenerado nunca se repita.
        builder.HasIndex(s => s.Correlativo).IsUnique();

        // Relación 1-1 con OrderDelivery: una orden tiene a lo sumo un envio.
        builder.HasOne(s => s.OrderDelivery)
            .WithOne(o => o.Shipment)
            .HasForeignKey<Shipment>(s => s.OrderDeliveryId)
            .IsRequired();

        builder.HasIndex(s => s.OrderDeliveryId).IsUnique();
    }
}
