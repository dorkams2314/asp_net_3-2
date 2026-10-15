using AutoMapper;
using Microsoft.EntityFrameworkCore;
using asp_net_3_2.Controllers;
using asp_net_3_2.DTOs.CartItems;
using asp_net_3_2.DTOs.Carts;
using asp_net_3_2.DTOs.Categories;
using asp_net_3_2.DTOs.OrderItems;
using asp_net_3_2.DTOs.Orders;
using asp_net_3_2.DTOs.Products;
using asp_net_3_2.DTOs.Roles;
using asp_net_3_2.DTOs.Users;
using asp_net_3_2.Mapping;
using asp_net_3_2.Models;
using asp_net_3_2.Repositories;
using asp_net_3_2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<ServiceExceptionFilter>());
builder.Services.AddAutoMapper(configuration =>
{
    configuration.AddProfile<MappingProfile>();
}, Array.Empty<System.Reflection.Assembly>(), ServiceLifetime.Scoped);
builder.Services.AddDbContext<AutoFixDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AutoFixDb")));
builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
builder.Services.AddScoped<IService<RoleDto, RoleWriteDto, RoleWriteDto, PatchRoleDto>, Service<Role, RoleDto, RoleWriteDto, RoleWriteDto, PatchRoleDto>>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IService<UserDto, UserWriteDto, UserWriteDto, PatchUserDto>, UserService>();
builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();
builder.Services.AddScoped<IService<CategoryDto, CategoryWriteDto, CategoryWriteDto, PatchCategoryDto>, Service<Category, CategoryDto, CategoryWriteDto, CategoryWriteDto, PatchCategoryDto>>();
builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IService<ProductDto, ProductWriteDto, ProductWriteDto, PatchProductDto>, ProductService>();
builder.Services.AddScoped<IRepository<Cart>, CartRepository>();
builder.Services.AddScoped<IService<CartDto, CartWriteDto, CartWriteDto, PatchCartDto>, CartService>();
builder.Services.AddScoped<IRepository<CartItem>, CartItemRepository>();
builder.Services.AddScoped<IService<CartItemDto, CartItemWriteDto, CartItemWriteDto, PatchCartItemDto>, CartItemService>();
builder.Services.AddScoped<IRepository<OrderItem>, OrderItemRepository>();
builder.Services.AddScoped<IService<OrderItemDto, OrderItemWriteDto, OrderItemWriteDto, PatchOrderItemDto>, OrderItemService>();
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
builder.Services.AddScoped<IService<OrderDto, CreateOrderDto, UpdateOrderDto, PatchOrderDto>, OrderService>();
builder.Services.AddScoped<IRepository<Reservation>, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>().AssertConfigurationIsValid();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
