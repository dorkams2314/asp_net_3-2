using asp_net_3_2.Domain;
using asp_net_3_2.DTOs;
using asp_net_3_2.Models;
using asp_net_3_2.Repositories;
using AutoMapper;

namespace asp_net_3_2.Services;

public class Service<TEntity, TDto, TCreate, TUpdate, TPatch>(IRepository<TEntity> repository, IMapper mapper)
    : IService<TDto, TCreate, TUpdate, TPatch> where TEntity : class, IEntity
{
    protected IRepository<TEntity> Repository { get; } = repository;

    protected IMapper Mapper { get; } = mapper;

    public async Task<PageResult<TDto>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var result = await Repository.GetPageAsync(page, pageSize, cancellationToken);
        return new PageResult<TDto>(Mapper.Map<List<TDto>>(result.Items), result.TotalCount);
    }

    public async Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException($"{typeof(TEntity).Name} {id} was not found.");
        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> CreateAsync(TCreate dto, CancellationToken cancellationToken)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await ValidateAsync(entity, cancellationToken);
        Repository.Add(entity);
        await Repository.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken);
    }

    public virtual async Task UpdateAsync(int id, TUpdate dto, CancellationToken cancellationToken)
    {
        var entity = await FindRequiredAsync(id, cancellationToken);
        Mapper.Map(dto, entity);
        await ValidateAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task PatchAsync(int id, TPatch dto, CancellationToken cancellationToken)
    {
        var entity = await FindRequiredAsync(id, cancellationToken);
        Mapper.Map(dto, entity);
        await ValidateAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await FindRequiredAsync(id, cancellationToken);
        Repository.Remove(entity);
        await Repository.SaveChangesAsync(cancellationToken);
    }

    protected async Task<TEntity> FindRequiredAsync(int id, CancellationToken cancellationToken) =>
        await Repository.FindAsync(id, cancellationToken)
        ?? throw new EntityNotFoundException($"{typeof(TEntity).Name} {id} was not found.");

    protected virtual Task ValidateAsync(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;

    protected static async Task RequireExistsAsync<TRelated>(IRepository<TRelated> repository, int id,
        CancellationToken cancellationToken) where TRelated : class, IEntity
    {
        if (!await repository.ExistsAsync(entity => entity.Id == id, cancellationToken))
        {
            throw new EntityNotFoundException($"{typeof(TRelated).Name} {id} was not found.");
        }
    }
}
