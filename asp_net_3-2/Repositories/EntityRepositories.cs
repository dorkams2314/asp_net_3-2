using asp_net_3_2.Models;
using Microsoft.EntityFrameworkCore;

namespace asp_net_3_2.Repositories;

public sealed class RoleRepository(AutoFixDbContext context) : Repository<Role>(context)
{
    protected override IQueryable<Role> ReadQuery => Context.Roles.Include(role => role.Users);
}

public sealed class UserRepository(AutoFixDbContext context) : Repository<User>(context)
{
    protected override IQueryable<User> ReadQuery => Context.Users
        .Include(user => user.Role).Include(user => user.Carts).Include(user => user.Orders);
}

public sealed class CategoryRepository(AutoFixDbContext context) : Repository<Category>(context)
{
    protected override IQueryable<Category> ReadQuery => Context.Categories.Include(category => category.Products);
}

public sealed class ProductRepository(AutoFixDbContext context) : Repository<Product>(context)
{
    protected override IQueryable<Product> ReadQuery => Context.Products.Include(product => product.Category);
}

public sealed class CartRepository(AutoFixDbContext context) : Repository<Cart>(context)
{
    protected override IQueryable<Cart> ReadQuery => Context.Carts
        .Include(cart => cart.User).Include(cart => cart.CartItems).ThenInclude(item => item.Product);
}

public sealed class CartItemRepository(AutoFixDbContext context) : Repository<CartItem>(context)
{
    protected override IQueryable<CartItem> ReadQuery => Context.CartItems
        .Include(item => item.Cart).Include(item => item.Product);
}

public sealed class OrderRepository(AutoFixDbContext context) : Repository<Order>(context)
{
    protected override IQueryable<Order> ReadQuery => Context.Orders
        .Include(order => order.User).Include(order => order.OrderItems).ThenInclude(item => item.Product);

    public override void Remove(Order entity)
    {
        Context.OrderItems.RemoveRange(Context.OrderItems.Where(item => item.OrderId == entity.Id));
        base.Remove(entity);
    }
}

public sealed class OrderItemRepository(AutoFixDbContext context) : Repository<OrderItem>(context)
{
    protected override IQueryable<OrderItem> ReadQuery => Context.OrderItems
        .Include(item => item.Order).Include(item => item.Product);
}

public sealed class ReservationRepository(AutoFixDbContext context) : Repository<Reservation>(context)
{
    protected override IQueryable<Reservation> ReadQuery => Context.Reservations
        .Include(reservation => reservation.Order)
        .Include(reservation => reservation.User)
        .Include(reservation => reservation.Product);
}
