using asp_net_3_2.DTOs;

namespace asp_net_3_2.Services;

public interface IService<TDto, in TCreate, in TUpdate, in TPatch>
{
    Task<PageResult<TDto>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TDto> CreateAsync(TCreate dto, CancellationToken cancellationToken);

    Task UpdateAsync(int id, TUpdate dto, CancellationToken cancellationToken);

    Task PatchAsync(int id, TPatch dto, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
