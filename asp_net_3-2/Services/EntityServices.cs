using asp_net_3_2.DTOs.CartItems;
using asp_net_3_2.DTOs.Carts;
using asp_net_3_2.DTOs.OrderItems;
using asp_net_3_2.DTOs.Orders;
using asp_net_3_2.DTOs.Products;
using asp_net_3_2.DTOs.Users;
using asp_net_3_2.Models;
using asp_net_3_2.Repositories;
using AutoMapper;

namespace asp_net_3_2.Services;

public sealed class UserService(IRepository<User> repository, IRepository<Role> roles, IMapper mapper)
    : Service<User, UserDto, UserWriteDto, UserWriteDto, PatchUserDto>(repository, mapper)
{
    protected override Task ValidateAsync(User entity, CancellationToken cancellationToken) =>
        RequireExistsAsync(roles, entity.RoleId, cancellationToken);
}

public sealed class ProductService(IRepository<Product> repository, IRepository<Category> categories, IMapper mapper)
    : Service<Product, ProductDto, ProductWriteDto, ProductWriteDto, PatchProductDto>(repository, mapper)
{
    protected override Task ValidateAsync(Product entity, CancellationToken cancellationToken) =>
        RequireExistsAsync(categories, entity.CategoryId, cancellationToken);
}

public sealed class CartService(IRepository<Cart> repository, IRepository<User> users, IMapper mapper)
    : Service<Cart, CartDto, CartWriteDto, CartWriteDto, PatchCartDto>(repository, mapper)
{
    protected override Task ValidateAsync(Cart entity, CancellationToken cancellationToken) =>
        RequireExistsAsync(users, entity.UserId, cancellationToken);
}

public sealed class CartItemService(IRepository<CartItem> repository, IRepository<Cart> carts,
    IRepository<Product> products, IMapper mapper)
    : Service<CartItem, CartItemDto, CartItemWriteDto, CartItemWriteDto, PatchCartItemDto>(repository, mapper)
{
    protected override async Task ValidateAsync(CartItem entity, CancellationToken cancellationToken)
    {
        await RequireExistsAsync(carts, entity.CartId, cancellationToken);
        await RequireExistsAsync(products, entity.ProductId, cancellationToken);
    }
}

public sealed class OrderService(IRepository<Order> repository, IRepository<User> users, IMapper mapper)
    : Service<Order, OrderDto, CreateOrderDto, UpdateOrderDto, PatchOrderDto>(repository, mapper)
{
    protected override Task ValidateAsync(Order entity, CancellationToken cancellationToken) =>
        RequireExistsAsync(users, entity.UserId, cancellationToken);
}

public sealed class OrderItemService(IRepository<OrderItem> repository, IRepository<Order> orders,
    IRepository<Product> products, IMapper mapper)
    : Service<OrderItem, OrderItemDto, OrderItemWriteDto, OrderItemWriteDto, PatchOrderItemDto>(repository, mapper)
{
    protected override async Task ValidateAsync(OrderItem entity, CancellationToken cancellationToken)
    {
        await RequireExistsAsync(orders, entity.OrderId, cancellationToken);
        await RequireExistsAsync(products, entity.ProductId, cancellationToken);
    }
}
