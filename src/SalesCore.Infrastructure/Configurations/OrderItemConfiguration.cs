using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesCore.Domain.Orders;

namespace SalesCore.Infrastructure.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(c => c.Id);

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(oi => oi.OrderId);
    }
}