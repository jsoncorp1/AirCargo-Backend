namespace AC.Domain.Persistence;

public interface ICoreUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
