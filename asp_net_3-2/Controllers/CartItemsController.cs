using asp_net_3_2.DTOs.CartItems;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Route("api/cart-items")]
public sealed class CartItemsController(IService<CartItemDto, CartItemWriteDto, CartItemWriteDto, PatchCartItemDto> service)
    : CrudController<CartItemDto, CartItemWriteDto, CartItemWriteDto, PatchCartItemDto>(service)
{
}
