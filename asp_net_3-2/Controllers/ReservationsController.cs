using asp_net_3_2.DTOs.Reservations;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Route("api/reservations")]
public sealed class ReservationsController(IReservationService service)
    : CrudController<ReservationDto, CreateReservationDto, UpdateReservationDto, PatchReservationDto>(service)
{
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        await service.CancelAsync(id, cancellationToken);
        return NoContent();
    }
}
