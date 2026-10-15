namespace asp_net_3_2.DTOs.CartItems;

public sealed class CartItemDto : IResourceDto
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public CartSummaryDto Cart { get; set; } = null!;

    public ProductSummaryDto Product { get; set; } = null!;
}
