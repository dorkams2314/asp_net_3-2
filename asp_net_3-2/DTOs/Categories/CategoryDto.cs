namespace asp_net_3_2.DTOs.Categories;

public sealed class CategoryDto : IResourceDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public IReadOnlyList<ProductSummaryDto> Products { get; set; } = [];
}
