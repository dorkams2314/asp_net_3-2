using asp_net_3_2.DTOs;
using asp_net_3_2.DTOs.Roles;
using asp_net_3_2.DTOs.Users;
using asp_net_3_2.DTOs.Categories;
using asp_net_3_2.DTOs.Products;
using asp_net_3_2.DTOs.Carts;
using asp_net_3_2.DTOs.CartItems;
using asp_net_3_2.DTOs.OrderItems;
using asp_net_3_2.DTOs.Orders;
using asp_net_3_2.DTOs.Reservations;
using asp_net_3_2.Models;
using AutoMapper;

namespace asp_net_3_2.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Role, RoleDto>();
        CreateMap<User, UserDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<Product, ProductDto>();
        CreateMap<Cart, CartDto>();
        CreateMap<CartItem, CartItemDto>();
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<Order, OrderDto>()
            .ForMember(dto => dto.CreatedAt, options => options.MapFrom(entity => entity.OrderDate));
        CreateMap<Reservation, ReservationDto>();
        CreateMap<Role, RoleSummaryDto>();
        CreateMap<Category, CategorySummaryDto>();
        CreateMap<User, UserSummaryDto>();
        CreateMap<Product, ProductSummaryDto>();
        CreateMap<Cart, CartSummaryDto>();
        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dto => dto.CreatedAt, options => options.MapFrom(entity => entity.OrderDate));

        CreateMap<RoleWriteDto, Role>(MemberList.Source);
        CreateMap<PatchRoleDto, Role>(MemberList.Source)
            .ForMember(entity => entity.Name, options => options.PreCondition(dto => dto.Name != null));
        CreateMap<UserWriteDto, User>(MemberList.Source);
        CreateMap<PatchUserDto, User>(MemberList.Source)
            .ForMember(entity => entity.Login, options => options.PreCondition(dto => dto.Login != null))
            .ForMember(entity => entity.UserName, options => options.PreCondition(dto => dto.UserName != null))
            .ForMember(entity => entity.PasswordHash, options => options.PreCondition(dto => dto.PasswordHash != null))
            .ForMember(entity => entity.IsActive, options => options.PreCondition(dto => dto.IsActive != null))
            .ForMember(entity => entity.RoleId, options => options.PreCondition(dto => dto.RoleId != null));
        CreateMap<CategoryWriteDto, Category>(MemberList.Source);
        CreateMap<PatchCategoryDto, Category>(MemberList.Source)
            .ForMember(entity => entity.Name, options => options.PreCondition(dto => dto.Name != null));
        CreateMap<ProductWriteDto, Product>(MemberList.Source);
        CreateMap<PatchProductDto, Product>(MemberList.Source)
            .ForMember(entity => entity.Name, options => options.PreCondition(dto => dto.Name != null))
            .ForMember(entity => entity.Description, options => options.PreCondition(dto => dto.Description != null))
            .ForMember(entity => entity.ImagePath, options => options.PreCondition(dto => dto.ImagePath != null))
            .ForMember(entity => entity.Price, options => options.PreCondition(dto => dto.Price != null))
            .ForMember(entity => entity.StockQuantity, options => options.PreCondition(dto => dto.StockQuantity != null))
            .ForMember(entity => entity.CategoryId, options => options.PreCondition(dto => dto.CategoryId != null));
        CreateMap<CartWriteDto, Cart>(MemberList.Source);
        CreateMap<PatchCartDto, Cart>(MemberList.Source)
            .ForMember(entity => entity.UserId, options => options.PreCondition(dto => dto.UserId != null));
        CreateMap<CartItemWriteDto, CartItem>(MemberList.Source);
        CreateMap<PatchCartItemDto, CartItem>(MemberList.Source)
            .ForMember(entity => entity.CartId, options => options.PreCondition(dto => dto.CartId != null))
            .ForMember(entity => entity.ProductId, options => options.PreCondition(dto => dto.ProductId != null))
            .ForMember(entity => entity.Quantity, options => options.PreCondition(dto => dto.Quantity != null));
        CreateMap<OrderItemWriteDto, OrderItem>(MemberList.Source);
        CreateMap<PatchOrderItemDto, OrderItem>(MemberList.Source)
            .ForMember(entity => entity.OrderId, options => options.PreCondition(dto => dto.OrderId != null))
            .ForMember(entity => entity.ProductId, options => options.PreCondition(dto => dto.ProductId != null))
            .ForMember(entity => entity.Quantity, options => options.PreCondition(dto => dto.Quantity != null))
            .ForMember(entity => entity.Price, options => options.PreCondition(dto => dto.Price != null));
        CreateMap<CreateOrderDto, Order>(MemberList.Source)
            .ForMember(entity => entity.OrderDate, options => options.MapFrom(_ => DateTime.UtcNow));
        CreateMap<UpdateOrderDto, Order>(MemberList.Source);
        CreateMap<PatchOrderDto, Order>(MemberList.Source)
            .ForMember(entity => entity.UserId, options => options.PreCondition(dto => dto.UserId != null));
        CreateMap<CreateReservationDto, Reservation>(MemberList.Source)
            .ForMember(entity => entity.CreatedAt, options => options.MapFrom(_ => DateTime.UtcNow));
        CreateMap<UpdateReservationDto, Reservation>(MemberList.Source);
        CreateMap<PatchReservationDto, Reservation>(MemberList.Source)
            .ForMember(entity => entity.Quantity, options => options.PreCondition(dto => dto.Quantity != null));
    }
}
