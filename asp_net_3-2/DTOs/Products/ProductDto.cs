namespace asp_net_3_2.DTOs.Products;

public sealed class ProductDto : IResourceDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int CategoryId { get; set; }

    public CategorySummaryDto Category { get; set; } = null!;
}
