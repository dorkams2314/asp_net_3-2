using Microsoft.AspNetCore.Authorization;
using asp_net_3_2.Security;
using asp_net_3_2.DTOs.Orders;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Authorize(Policy = Policies.Admin)]
[Route("api/orders")]
public sealed class OrdersController(IService<OrderDto, CreateOrderDto, UpdateOrderDto, PatchOrderDto> service)
    : CrudController<OrderDto, CreateOrderDto, UpdateOrderDto, PatchOrderDto>(service)
{
}
