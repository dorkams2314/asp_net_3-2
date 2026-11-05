using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Reservations;

public sealed class CreateReservationDto
{
    [Range(1, int.MaxValue)]
    public required int OrderId { get; set; }

    [Range(1, int.MaxValue)]
    public required int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public required int Quantity { get; set; }
}
