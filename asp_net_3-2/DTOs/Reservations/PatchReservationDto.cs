using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Reservations;

public sealed class PatchReservationDto
{
    [Range(1, int.MaxValue)]
    public int? Quantity { get; set; }
}
