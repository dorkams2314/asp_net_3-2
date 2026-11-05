using asp_net_3_2.Models;

namespace asp_net_3_2.Repositories;

public interface IAccountRepository : IRepository<User>
{
    Task<User?> FindByLoginAsync(string login, CancellationToken cancellationToken);
    Task<User?> FindAccountAsync(int id, CancellationToken cancellationToken);
    Task<Role> GetUserRoleAsync(CancellationToken cancellationToken);
}
