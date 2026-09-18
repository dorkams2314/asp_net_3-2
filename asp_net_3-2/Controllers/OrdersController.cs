using asp_net_3_2.DTOs.Orders;
using asp_net_3_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asp_net_3_2.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly AutoFixDbContext _context;

    public OrdersController(AutoFixDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrderDto>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<OrderDto>> GetAll(int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var orders = _context.Orders
            .AsNoTracking()
            .OrderBy(order => order.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(order => ToDto(order))
            .ToList();

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<OrderDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<OrderDto> GetById(int id)
    {
        var order = _context.Orders
            .AsNoTracking()
            .FirstOrDefault(order => order.Id == id);

        return order is null ? NotFound() : Ok(ToDto(order));
    }

    [HttpPost]
    [ProducesResponseType<OrderDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<OrderDto> Create(CreateOrderDto dto)
    {
        if (!_context.Users.Any(user => user.Id == dto.UserId))
        {
            return NotFound();
        }

        var order = new Order
        {
            UserId = dto.UserId,
            OrderDate = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, ToDto(order));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, UpdateOrderDto dto)
    {
        var order = _context.Orders.FirstOrDefault(order => order.Id == id);
        if (order is null || !_context.Users.Any(user => user.Id == dto.UserId))
        {
            return NotFound();
        }

        order.UserId = dto.UserId;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Patch(int id, PatchOrderDto dto)
    {
        var order = _context.Orders.FirstOrDefault(order => order.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        if (dto.UserId is int userId)
        {
            if (!_context.Users.Any(user => user.Id == userId))
            {
                return NotFound();
            }

            order.UserId = userId;
        }

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var order = _context.Orders
            .Include(order => order.OrderItems)
            .FirstOrDefault(order => order.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);
        _context.SaveChanges();

        return NoContent();
    }

    private static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            CreatedAt = order.OrderDate
        };
    }
}
