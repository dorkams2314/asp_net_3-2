using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Users;

public sealed class PatchUserDto
{
    [MinLength(1), StringLength(100)]
    public string? Login { get; set; }

    [MinLength(1), StringLength(100)]
    public string? UserName { get; set; }

    [MinLength(1), StringLength(256)]
    public string? PasswordHash { get; set; }

    public bool? IsActive { get; set; }

    [Range(1, int.MaxValue)]
    public int? RoleId { get; set; }
}
