using AC.Domain.Modules.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfShipmentDetailConfig : IEntityTypeConfiguration<ShipmentDetail>
{
    public void Configure(EntityTypeBuilder<ShipmentDetail> builder)
    {
        builder.Property(d => d.Weight)
            .HasPrecision(18, 2);

        builder.Property(d => d.ShippingCost)
            .HasPrecision(18, 2);
    }
}
