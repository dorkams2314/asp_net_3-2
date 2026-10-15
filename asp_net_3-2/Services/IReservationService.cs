using asp_net_3_2.DTOs.Reservations;

namespace asp_net_3_2.Services;

public interface IReservationService
    : IService<ReservationDto, CreateReservationDto, UpdateReservationDto, PatchReservationDto>
{
    Task CancelAsync(int id, CancellationToken cancellationToken);
}
