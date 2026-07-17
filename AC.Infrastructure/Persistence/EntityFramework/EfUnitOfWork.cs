using AC.Domain.Persistence;   

namespace AC.Infrastructure.Persistence.EntityFramework;

public class EfUnitOfWork(CoreDbContext coreDbContext) : ICoreUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}