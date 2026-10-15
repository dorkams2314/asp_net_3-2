using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.CartItems;

public sealed class PatchCartItemDto
{
    [Range(1, int.MaxValue)]
    public int? CartId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int? Quantity { get; set; }
}
