using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {

        public string BuyerEmail { get; set; } = null!;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; } = null!;
        public int? DeliveryMethodId { get; set; }//Foreign Key
        public DeliveryMethod? DeliveryMethod { get; set; } = null!;//Navigational Prop [One]
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();//Navigational Property [Many]
        public decimal SubTotal { get; set; }//Cost Of Order With it's all items without shipping cost
        //[NotMapped]
        //public decimal Total => SubTotal + DeliveryMethod.Cost;//Read Only Attribute
        public decimal GetTotal() => SubTotal + DeliveryMethod?.Cost ?? 0;
        public string? PaymentIntentId { get; set; }

        //For EF Core
        private Order()
        {
        }
        public Order(string buyerEmail, Address shippingAddress, int? deliveryMethodId, ICollection<OrderItem> items, decimal subTotal,string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethodId = deliveryMethodId;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

    }
}
