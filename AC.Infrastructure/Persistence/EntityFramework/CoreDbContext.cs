using AC.Domain.Modules.Articles;
using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Modules.Users;
using Microsoft.EntityFrameworkCore;

namespace AC.Infrastructure.Persistence.EntityFramework;

public class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleReceipt> ArticleReceipts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CoreDbContext).Assembly,
            t => t.Namespace != null &&
                 t.Namespace.StartsWith("AC.Infrastructure.Persistence.EntityFramework.Configurations"));

        // Igual que en EDI: nada de borrado en cascada por defecto
        foreach (var foreignKey in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        base.OnModelCreating(modelBuilder);
    }
}