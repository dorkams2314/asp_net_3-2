using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Reservations;

public sealed class UpdateReservationDto
{
    [Range(1, int.MaxValue)]
    public required int Quantity { get; set; }
}
