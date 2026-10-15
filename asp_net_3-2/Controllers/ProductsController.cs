using asp_net_3_2.DTOs.Products;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Route("api/products")]
public sealed class ProductsController(IService<ProductDto, ProductWriteDto, ProductWriteDto, PatchProductDto> service)
    : CrudController<ProductDto, ProductWriteDto, ProductWriteDto, PatchProductDto>(service)
{
}
