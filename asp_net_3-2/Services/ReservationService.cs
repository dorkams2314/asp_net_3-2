using asp_net_3_2.Domain;
using asp_net_3_2.DTOs.Reservations;
using asp_net_3_2.Models;
using asp_net_3_2.Repositories;
using AutoMapper;

namespace asp_net_3_2.Services;

public sealed class ReservationService(IRepository<Reservation> repository, IRepository<Product> products,
    IRepository<Order> orders, IRepository<User> users, IMapper mapper)
    : Service<Reservation, ReservationDto, CreateReservationDto, UpdateReservationDto, PatchReservationDto>(repository, mapper),
        IReservationService
{
    public override async Task<ReservationDto> CreateAsync(CreateReservationDto dto, CancellationToken cancellationToken)
    {
        var order = await orders.FindAsync(dto.OrderId, cancellationToken)
            ?? throw new EntityNotFoundException($"Order {dto.OrderId} was not found.");
        var user = await users.FindAsync(order.UserId, cancellationToken)
            ?? throw new EntityNotFoundException($"User {order.UserId} was not found.");
        if (!user.IsActive)
        {
            throw new ConflictException("Products cannot be reserved for an inactive user.");
        }

        var product = await products.FindAsync(dto.ProductId, cancellationToken)
            ?? throw new EntityNotFoundException($"Product {dto.ProductId} was not found.");
        if (product.StockQuantity < dto.Quantity)
        {
            throw new ConflictException($"Insufficient stock. Available: {product.StockQuantity}, requested: {dto.Quantity}.");
        }

        var reservation = Mapper.Map<Reservation>(dto);
        reservation.UserId = order.UserId;
        product.StockQuantity -= reservation.Quantity;
        Repository.Add(reservation);
        await Repository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(reservation.Id, cancellationToken);
    }

    public async Task CancelAsync(int id, CancellationToken cancellationToken)
    {
        var reservation = await FindRequiredAsync(id, cancellationToken);
        if (reservation.CancelledAt is not null)
        {
            throw new EntityNotFoundException($"Active reservation {id} was not found.");
        }

        var product = await products.FindAsync(reservation.ProductId, cancellationToken)
            ?? throw new EntityNotFoundException($"Product {reservation.ProductId} was not found.");
        var restoredStock = (long)product.StockQuantity + reservation.Quantity;
        if (restoredStock > int.MaxValue)
        {
            throw new ConflictException("Returning this reservation would exceed the maximum stock quantity.");
        }

        product.StockQuantity = (int)restoredStock;
        reservation.CancelledAt = DateTime.UtcNow;
        await Repository.SaveChangesAsync(cancellationToken);
    }

    public override Task UpdateAsync(int id, UpdateReservationDto dto, CancellationToken cancellationToken) =>
        ChangeQuantityAsync(id, dto, cancellationToken);

    public override Task PatchAsync(int id, PatchReservationDto dto, CancellationToken cancellationToken) =>
        ChangeQuantityAsync(id, dto, cancellationToken);

    public override async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var reservation = await FindRequiredAsync(id, cancellationToken);
        if (reservation.CancelledAt is null)
        {
            throw new ConflictException("Cancel the active reservation before deleting it.");
        }

        Repository.Remove(reservation);
        await Repository.SaveChangesAsync(cancellationToken);
    }

    private async Task ChangeQuantityAsync<TRequest>(int id, TRequest dto, CancellationToken cancellationToken)
    {
        var reservation = await FindRequiredAsync(id, cancellationToken);
        if (reservation.CancelledAt is not null)
        {
            throw new ConflictException("A cancelled reservation cannot be changed.");
        }

        var previousQuantity = reservation.Quantity;
        Mapper.Map(dto, reservation);
        var product = await products.FindAsync(reservation.ProductId, cancellationToken)
            ?? throw new EntityNotFoundException($"Product {reservation.ProductId} was not found.");
        var availableStock = (long)product.StockQuantity + previousQuantity - reservation.Quantity;
        if (availableStock < 0)
        {
            throw new ConflictException($"Insufficient stock. Available additional quantity: {product.StockQuantity}.");
        }

        if (availableStock > int.MaxValue)
        {
            throw new ConflictException("Reducing this reservation would exceed the maximum stock quantity.");
        }

        product.StockQuantity = (int)availableStock;
        await Repository.SaveChangesAsync(cancellationToken);
    }
}
