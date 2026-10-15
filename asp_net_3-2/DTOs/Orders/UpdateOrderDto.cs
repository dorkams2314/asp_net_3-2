using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Orders;

public sealed class UpdateOrderDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}
