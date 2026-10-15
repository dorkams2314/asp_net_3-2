using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Carts;

public sealed class PatchCartDto
{
    [Range(1, int.MaxValue)]
    public int? UserId { get; set; }
}
