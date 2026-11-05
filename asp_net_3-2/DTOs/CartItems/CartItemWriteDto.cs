using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.CartItems;

public sealed class CartItemWriteDto
{
    [Range(1, int.MaxValue)]
    public required int CartId { get; set; }

    [Range(1, int.MaxValue)]
    public required int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public required int Quantity { get; set; }
}
