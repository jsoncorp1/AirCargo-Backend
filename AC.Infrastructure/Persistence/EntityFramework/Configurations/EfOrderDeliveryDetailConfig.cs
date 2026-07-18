using AC.Domain.Modules.OrderDeliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfOrderDeliveryDetailConfig : IEntityTypeConfiguration<OrderDeliveryDetail>
{
    public void Configure(EntityTypeBuilder<OrderDeliveryDetail> builder)
    {
        builder.Property(d => d.ArticleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(d => d.LineTotal)
            .HasPrecision(18, 2);
    }
}
