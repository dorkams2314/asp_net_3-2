using asp_net_3_2.DTOs.Carts;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Route("api/carts")]
public sealed class CartsController(IService<CartDto, CartWriteDto, CartWriteDto, PatchCartDto> service)
    : CrudController<CartDto, CartWriteDto, CartWriteDto, PatchCartDto>(service)
{
}
