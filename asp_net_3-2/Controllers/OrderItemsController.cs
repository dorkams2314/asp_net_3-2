using Microsoft.AspNetCore.Authorization;
using asp_net_3_2.Security;
using asp_net_3_2.DTOs.OrderItems;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Authorize(Policy = Policies.Admin)]
[Route("api/order-items")]
public sealed class OrderItemsController(IService<OrderItemDto, OrderItemWriteDto, OrderItemWriteDto, PatchOrderItemDto> service)
    : CrudController<OrderItemDto, OrderItemWriteDto, OrderItemWriteDto, PatchOrderItemDto>(service)
{
}
