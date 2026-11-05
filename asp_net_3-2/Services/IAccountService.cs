using asp_net_3_2.DTOs.Auth;
using asp_net_3_2.DTOs.Users;
using asp_net_3_2.DTOs;

namespace asp_net_3_2.Services;

public interface IAccountService
{
    Task<AuthResponse> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);
    Task<AuthResponse?> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
    Task<UserDto> GetAsync(int id, CancellationToken cancellationToken);
    Task<PageResult<UserDto>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task UpdateProfileAsync(int id, UpdateProfileDto dto, CancellationToken cancellationToken);
}
