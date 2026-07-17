using AC.Domain.Persistence;
using AC.Infrastructure.Persistence.EntityFramework.Repositories;
using AC.Infrastructure.Persistence.EntityFramework.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AC.Infrastructure.Persistence.EntityFramework;

public static class AddCustomDbContextMethod
{
    public static void AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<CoreDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("AirCargoDb")!);
            options.UseSnakeCaseNamingConvention();
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);

        services.AddScoped<RoleSeeder>();
        services.AddScoped<UserSeeder>();
        services.AddScoped<ICoreUnitOfWork, EfUnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    }
}