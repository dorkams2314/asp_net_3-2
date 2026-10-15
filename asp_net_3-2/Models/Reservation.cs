namespace asp_net_3_2.Models;

public sealed class Reservation : IEntity
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public User User { get; set; } = null!;
}
