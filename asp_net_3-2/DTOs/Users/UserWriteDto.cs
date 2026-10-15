using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Users;

public sealed class UserWriteDto
{
    [Required, StringLength(100)]
    public string Login { get; set; } = null!;

    [Required, StringLength(100)]
    public string UserName { get; set; } = null!;

    [Required, StringLength(256)]
    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }
}
