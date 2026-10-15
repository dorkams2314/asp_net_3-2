using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.OrderItems;

public sealed class OrderItemWriteDto
{
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }

    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal Price { get; set; }
}
