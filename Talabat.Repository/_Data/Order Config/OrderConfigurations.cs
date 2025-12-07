using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Infrastructure._Data.Order_Config
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(Order => Order.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner());//Composite Attribute - Not Map the prop [ShippingAddress] as seperate table, map it's properties inside Order table.
            builder.Property(order => order.Status)
                   .HasConversion(
                (OrderStatus) => OrderStatus.ToString(),
                (OrderStatus) => (OrderStatus)Enum.Parse(typeof(OrderStatus), OrderStatus)
                );//To Store it as string in DB and when returned returned as OrderStatus integer value

            builder.Property(order => order.SubTotal)
                   .HasColumnType("decimal(12,2)");
        }
    }
}
