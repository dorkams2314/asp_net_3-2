using asp_net_3_2.DTOs.OrderItems;

namespace asp_net_3_2.DTOs.Orders;

public sealed class OrderDto : IResourceDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserSummaryDto User { get; set; } = null!;

    public IReadOnlyList<OrderItemDto> OrderItems { get; set; } = [];
}
