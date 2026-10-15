using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Roles;

public sealed class PatchRoleDto
{
    [MinLength(1), StringLength(50)]
    public string? Name { get; set; }
}
