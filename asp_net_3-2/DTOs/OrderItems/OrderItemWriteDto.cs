using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.OrderItems;

public sealed class OrderItemWriteDto
{
    [Range(1, int.MaxValue)]
    public required int OrderId { get; set; }

    [Range(1, int.MaxValue)]
    public required int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public required int Quantity { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public required decimal Price { get; set; }
}
