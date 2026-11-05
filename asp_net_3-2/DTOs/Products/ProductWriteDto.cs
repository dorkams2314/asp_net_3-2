using System.ComponentModel.DataAnnotations;

namespace asp_net_3_2.DTOs.Products;

public sealed class ProductWriteDto
{
    [Required, StringLength(200)]
    public required string Name { get; set; }

    [Required(AllowEmptyStrings = true), StringLength(500)]
    public required string Description { get; set; }

    [Required(AllowEmptyStrings = true), StringLength(300)]
    public required string ImagePath { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public required decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public required int StockQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public required int CategoryId { get; set; }
}
