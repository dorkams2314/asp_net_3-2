namespace asp_net_3_2.DTOs.Users;

public sealed class UserDto : IResourceDto
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public bool IsActive { get; set; }

    public int RoleId { get; set; }

    public RoleSummaryDto Role { get; set; } = null!;

    public IReadOnlyList<CartSummaryDto> Carts { get; set; } = [];

    public IReadOnlyList<OrderSummaryDto> Orders { get; set; } = [];
}
