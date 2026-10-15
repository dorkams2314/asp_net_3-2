using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Products;

public sealed class ProductWriteDto
{
    [Required, StringLength(200)]
    public string Name { get; set; } = null!;

    [Required(AllowEmptyStrings = true), StringLength(500)]
    public string Description { get; set; } = null!;

    [Required(AllowEmptyStrings = true), StringLength(300)]
    public string ImagePath { get; set; } = null!;

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
