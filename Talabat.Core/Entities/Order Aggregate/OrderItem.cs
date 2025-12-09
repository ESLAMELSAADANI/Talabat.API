using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    //The product that ordered as an item inside the CustomerOrder with specific price and quantity 
    public class OrderItem : BaseEntity
    {
        //For EF Core
        private OrderItem()
        {
        }

        public OrderItem(ProductItemOrdered product, decimal price, int quantity)
        {
            Product = product;
            Price = price;
            Quantity = quantity;
        }

        public ProductItemOrdered Product { get; set; } = null!;
        public decimal Price { get; set; }//Price of product as an item in the order not the productPrice - May be there is voucher or discount
        public int Quantity { get; set; }
    }
}
