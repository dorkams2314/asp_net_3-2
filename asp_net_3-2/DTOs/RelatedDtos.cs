namespace asp_net_3_2.DTOs;

public sealed class RoleSummaryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}

public sealed class CategorySummaryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}

public sealed class UserSummaryDto
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public bool IsActive { get; set; }
}

public sealed class ProductSummaryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }
}

public sealed class CartSummaryDto
{
    public int Id { get; set; }

    public int UserId { get; set; }
}

public sealed class OrderSummaryDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
