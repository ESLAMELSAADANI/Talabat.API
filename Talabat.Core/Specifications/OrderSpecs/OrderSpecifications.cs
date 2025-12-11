using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderSpecifications : BaseSpecifications<Order>
    {
        //This constructor will used to get all orders for specific user 
        public OrderSpecifications(string buyerEmail) : base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);

            SetOrderByDesc(O => O.OrderDate);

        }
        //This constructor will used to get specific order for specific user 
        public OrderSpecifications(int orderId, string buyerEmail) : base(O => O.BuyerEmail == buyerEmail && O.Id == orderId)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
        }
    }
}
