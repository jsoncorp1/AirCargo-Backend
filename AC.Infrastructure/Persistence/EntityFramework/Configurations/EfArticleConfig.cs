using AC.Domain.Modules.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfArticleConfig : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.Property(a => a.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Price)
            .HasPrecision(18, 2);

        // La relación N-1 con Supplier se arma sola por convención
        // (Article.SupplierId + Article.Supplier + Supplier.Articles); el delete
        // queda en Restrict por el loop global del CoreDbContext.
    }
}
