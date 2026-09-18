namespace asp_net_3_2.DTOs.Orders;

public sealed class OrderDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
