using System.Linq.Expressions;
using asp_net_3_2.DTOs;
using asp_net_3_2.Models;
using asp_net_3_2.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace asp_net_3_2.Repositories;

public class Repository<TEntity>(AutoFixDbContext context) : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected AutoFixDbContext Context { get; } = context;

    protected virtual IQueryable<TEntity> ReadQuery => Context.Set<TEntity>();

    public async Task<PageResult<TEntity>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await Context.Set<TEntity>().CountAsync(cancellationToken);
        var items = await ReadQuery.AsNoTracking().AsSplitQuery()
            .OrderBy(entity => entity.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<TEntity>(items, totalCount);
    }

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        ReadQuery.AsNoTracking().AsSplitQuery().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    public async Task<TEntity?> FindAsync(int id, CancellationToken cancellationToken) =>
        await Context.Set<TEntity>().FindAsync([id], cancellationToken);

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) =>
        Context.Set<TEntity>().AnyAsync(predicate, cancellationToken);

    public void Add(TEntity entity) => Context.Set<TEntity>().Add(entity);

    public virtual void Remove(TEntity entity) => Context.Set<TEntity>().Remove(entity);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("The data changed during this operation. Reload it and try again.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 547 or 2601 or 2627 })
        {
            throw new ConflictException("The operation conflicts with related records or database constraints.");
        }
    }
}
