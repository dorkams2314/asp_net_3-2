using asp_net_3_2.DTOs.CartItems;

namespace asp_net_3_2.DTOs.Carts;

public sealed class CartDto : IResourceDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public UserSummaryDto User { get; set; } = null!;

    public IReadOnlyList<CartItemDto> CartItems { get; set; } = [];
}
