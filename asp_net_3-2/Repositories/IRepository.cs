using System.Linq.Expressions;
using asp_net_3_2.DTOs;
using asp_net_3_2.Models;

namespace asp_net_3_2.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    Task<PageResult<TEntity>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TEntity?> FindAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    void Add(TEntity entity);

    void Remove(TEntity entity);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
