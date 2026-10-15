using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Categories;

public sealed class CategoryWriteDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = null!;
}
