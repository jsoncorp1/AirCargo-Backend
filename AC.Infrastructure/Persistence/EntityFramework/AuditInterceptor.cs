using AC.Application.Abstractions.Security;
using AC.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AC.Infrastructure.Persistence.EntityFramework;

public class AuditInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? context = eventData.Context;
        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        DateTime now = DateTime.UtcNow;

        // Fuera de un request HTTP autenticado (seeders, jobs) no hay
        // ICurrentUser que resolver; "system" queda como identidad de esos casos.
        string user = currentUser.Email ?? currentUser.UserId?.ToString() ?? "system";

        foreach (var entry in context.ChangeTracker.Entries<CoreEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = user;
                entry.Entity.Active = true;
            }
            else if (entry.State == EntityState.Modified)
            {
                if (!entry.Entity.Active)
                {
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = user;
                }
                else
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = user;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}