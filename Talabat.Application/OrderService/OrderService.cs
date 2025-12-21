using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;

namespace Talabat.Application.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        //private readonly IGenericRepository<Product> _productRepo;
        //private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
        //private readonly IGenericRepository<Order> _orderRepo;

        public OrderService(
            IBasketRepository basketRepo,//This Repo Deal With another DB -> In-Memory database -> not the StoreContext
                                         //IGenericRepository<Product> productRepo,
                                         //IGenericRepository<DeliveryMethod> deliveryMethodRepo,
                                         //IGenericRepository<Order> orderRepo,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService
            )
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            //_productRepo = productRepo;
            //_deliveryMethodRepo = deliveryMethodRepo;
            //_orderRepo = orderRepo;
        }

        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, Address shippingAddress, int deliveryMethodId)
        {
            // 1.Get Basket From Baskets Repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // 2. Get Selected Items at Basket From Products Repo
            var orderItems = new List<OrderItem>();

            if (basket?.Items?.Count > 0)
            {
                var productRepository = _unitOfWork.Repository<Product>();
                foreach (var item in basket.Items)
                {
                    //I get the actual product because i don't trust the user, he could send invalid date for name or picture URL.
                    //I trust only the Id of the item and the quantity bcz  it's the id of product and the quantity of this product he would to buy.

                    //var product = await _productRepo.GetAsync(item.Id);
                    //UOW => Unit Of Work
                    var product = await productRepository.GetByIdAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                    orderItems.Add(orderItem);
                }
            }
            // 3. Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 4. Get Delivery Method From DeliveryMethods Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // 5.Check that there are no orders with this payment intent exists before.
            ///bcz if this happen, i will have 2 orders with the same payment intent 
            ///so when pay one payment intent found that he take two orders not one
            ///we may have 2 orders with the same payment intent in case that there is exception after order created
            /// => var result = await _unitOfWork.CompleteAsync();
            /// so the order will not returned to enduser
            /// but the order saved in databse actually 
            /// so what the end user will make ?
            /// he will click on create order again
            /// so will create another order with the same payment intent
            /// so when pay/checkout, he will pay and recieve two orders
            /// but he actually need one order.
            /// so need to check if there is existing order with the same payment intent => delete it
            /// just one order for one paymentIntent
            
            var orderRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderWithPaymentIntentSpecifications(basket?.PaymentIntentId);
            var existingOrder = await orderRepo.GetByIdWithSpecAsync(spec);

            if (existingOrder is not null)
            {
                orderRepo.Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);//To update amount of existing order in case if it's amount still is amount of deleted|previous order.
            }

            // 6. Create Order
            var order = new Order(
                buyerEmail: buyerEmail,
                shippingAddress: shippingAddress,
                deliveryMethodId: deliveryMethodId,
                items: orderItems,
                subTotal: subTotal,
                paymentIntentId: basket?.PaymentIntentId ?? ""
                );

            //_orderRepo.Add(order);
            _unitOfWork.Repository<Order>().Add(order);

            // 7. Save To Database [TODO]
            var result = await _unitOfWork.CompleteAsync();

            // 8. Return The Order 
            if (result <= 0) return null;
            return order;
        }
        public async Task<Order?> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var orderRepo = _unitOfWork.Repository<Order>();

            var spec = new OrderSpecifications(orderId, buyerEmail);

            var order = await orderRepo.GetByIdWithSpecAsync(spec);

            return order;
        }
        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var ordersRepo = _unitOfWork.Repository<Order>();

            var spec = new OrderSpecifications(buyerEmail);

            var orders = await ordersRepo.GetAllWithSpecAsync(spec);

            return orders;
        }
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethodRepo = _unitOfWork.Repository<DeliveryMethod>();

            var deliveryMethods = await deliveryMethodRepo.GetAllAsync();

            return deliveryMethods;
        }
    }
}
