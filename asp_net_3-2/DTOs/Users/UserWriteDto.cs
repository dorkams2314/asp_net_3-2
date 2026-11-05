using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Users;

public sealed class UserWriteDto
{
    [Required, StringLength(100)]
    public required string Login { get; set; }

    [Required, StringLength(100)]
    public required string UserName { get; set; }

    [Required, StringLength(256)]
    public required string PasswordHash { get; set; }

    public required bool IsActive { get; set; }

    [Range(1, int.MaxValue)]
    public required int RoleId { get; set; }
}
