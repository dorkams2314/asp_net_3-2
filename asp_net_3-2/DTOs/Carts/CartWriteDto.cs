using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Carts;

public sealed class CartWriteDto
{
    [Range(1, int.MaxValue)]
    public required int UserId { get; set; }
}
