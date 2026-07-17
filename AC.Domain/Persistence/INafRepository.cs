using AC.Domain.Common;

namespace AC.Domain.Persistence;

public interface INafRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : NafEntity
{
}
