namespace asp_net_3_2.DTOs.OrderItems;

public sealed class OrderItemDto : IResourceDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public OrderSummaryDto Order { get; set; } = null!;

    public ProductSummaryDto Product { get; set; } = null!;
}
