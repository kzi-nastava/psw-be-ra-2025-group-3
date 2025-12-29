using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext: DbContext
    {
        public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasDefaultSchema("payments");


            modelBuilder.Entity<ShoppingCart>(builder =>
            {
                builder.ToTable("ShoppingCarts", "payments");

                builder.HasKey(c => c.Id);

                builder.Property(c => c.Id)
                       .ValueGeneratedOnAdd();

                builder.Property(c => c.TouristId)
                       .IsRequired();

                builder.Property(c => c.TotalPrice)
                       .IsRequired();

                builder.Property(c => c.Items)
                    .HasColumnType("jsonb")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<OrderItem>>(v, (JsonSerializerOptions?)null) ?? new List<OrderItem>()
                    )
                    .Metadata.SetValueComparer(
                        new ValueComparer<List<OrderItem>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()
                        )
                    );
            });
        }
    }
}
