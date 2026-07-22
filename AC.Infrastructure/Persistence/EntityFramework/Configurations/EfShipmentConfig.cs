using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfShipmentConfig : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.TotalWeight)
            .HasPrecision(18, 2);

        builder.Property(s => s.ShippingPrice)
            .HasPrecision(18, 2);

        // Correlativo único solo entre envíos activos: MaxAsync (usado para generarlo)
        // también solo mira filas activas, así que un envío cancelado no debe seguir
        // "ocupando" su número frente a uno nuevo.
        builder.HasIndex(s => s.SequenceNumber)
            .IsUnique()
            .HasFilter("active = true");

        // Relación N-1 con OrderDelivery: puede haber envíos históricos (cancelados)
        // para la misma orden; "a lo sumo un envío activo" lo aplica el índice único
        // parcial de abajo, no la cardinalidad de la relación.
        builder.HasOne(s => s.OrderDelivery)
            .WithMany(o => o.Shipments)
            .HasForeignKey(s => s.OrderDeliveryId)
            .IsRequired();

        // Único solo entre envíos activos: si se cancela un envío, la orden debe
        // poder recibir uno nuevo (mismo OrderDeliveryId) sin chocar contra el viejo.
        builder.HasIndex(s => s.OrderDeliveryId)
            .IsUnique()
            .HasFilter("active = true");
    }
}
