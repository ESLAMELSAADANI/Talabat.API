using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.Application.OrderService
{
    public class OrderService : IOrderService
    {
        public Task<Order> CreateOrderAsync(string buyerEmail, string basketId, Address shippingAddress, string deliveryMethodId)
        {
            throw new NotImplementedException();
        }
        public Task<Order> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            throw new NotImplementedException();
        }
        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }
        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
