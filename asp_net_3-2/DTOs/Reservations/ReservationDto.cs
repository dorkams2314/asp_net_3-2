namespace asp_net_3_2.DTOs.Reservations;

public sealed class ReservationDto : IResourceDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public OrderSummaryDto Order { get; set; } = null!;

    public ProductSummaryDto Product { get; set; } = null!;

    public UserSummaryDto User { get; set; } = null!;
}
