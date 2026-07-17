using AC.Domain.Common;

namespace AC.Domain.Persistence;

public interface ILogRepository<TEntity> : IBaseRepository<TEntity> where TEntity : LogEntity
{
}
