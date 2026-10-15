using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Roles;

public sealed class RoleWriteDto
{
    [Required, StringLength(50)]
    public string Name { get; set; } = null!;
}
