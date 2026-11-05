using System.ComponentModel.DataAnnotations;
using asp_net_3_2.DTOs.Users;

namespace asp_net_3_2.DTOs.Auth;

public sealed record LoginDto(
    [Required, StringLength(100), RegularExpression(@"[a-zA-Z0-9._-]+")] string Login,
    [Required, StringLength(128, MinimumLength = 8)] string Password);

public sealed record RegisterDto(
    [Required, StringLength(100), RegularExpression(@"[a-zA-Z0-9._-]+")] string Login,
    [Required, StringLength(100)] string UserName,
    [Required, StringLength(128, MinimumLength = 8)] string Password);

public sealed record UpdateProfileDto([Required, StringLength(100)] string UserName);
public sealed record AuthResponse(string AccessToken, DateTime ExpiresAt, UserDto User);
