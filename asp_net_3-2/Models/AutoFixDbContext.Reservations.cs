using Microsoft.EntityFrameworkCore;

namespace asp_net_3_2.Models;

public partial class AutoFixDbContext
{
    public DbSet<Reservation> Reservations { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.StockQuantity).HasDefaultValue(0).IsConcurrencyToken();
            entity.ToTable(table => table.HasCheckConstraint("CK_Products_StockQuantity", "[StockQuantity] >= 0"));
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(reservation => reservation.Id);
            entity.Property(reservation => reservation.Quantity).IsConcurrencyToken();
            entity.Property(reservation => reservation.CancelledAt).IsConcurrencyToken();
            entity.HasOne(reservation => reservation.Order).WithMany()
                .HasForeignKey(reservation => reservation.OrderId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(reservation => reservation.Product).WithMany()
                .HasForeignKey(reservation => reservation.ProductId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(reservation => reservation.User).WithMany()
                .HasForeignKey(reservation => reservation.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.ToTable("Reservations", table =>
            {
                table.HasCheckConstraint("CK_Reservations_Quantity", "[Quantity] > 0");
                table.HasCheckConstraint("CK_Reservations_Dates", "[CancelledAt] IS NULL OR [CancelledAt] >= [CreatedAt]");
            });
        });
    }
}
