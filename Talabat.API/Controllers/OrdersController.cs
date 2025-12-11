using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.API.Controllers
{
    //[Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]// POST: /api/Orders
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [EndpointSummary("Create an order")]
        public async Task<ActionResult<Order>> CreateOrder(OrderDTO orderDTO)
        {
            //var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var shippingAddress = _mapper.Map<Address>(orderDTO.ShippingAddress);
            var order = await _orderService.CreateOrderAsync(orderDTO.BuyerEmail, orderDTO.BasketId, shippingAddress, orderDTO.DeliveryMethodId);
            if (order is not null)
                return Ok(order);
            return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest));
        }

        [HttpGet]// GET : /api/orders?email=eslamelsaadany7@outlook.com
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [EndpointSummary("Get orders for specific user by email.")]
        public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser(string email)//UserEmail
        {
            var orders = await _orderService.GetOrdersForUserAsync(email);
            return Ok(orders);
        }
        [HttpGet("{id}")]// GET : /api/orders/{orderId}?email=eslamelsaadany@outlook.com
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [EndpointSummary("Get specific order for user by order id and buyer|user email.")]
        public async Task<ActionResult<Order>> GetOrderForUser(int id, string email)//Order Id,buyerEmail
        {
            var order = await _orderService.GetOrderByIdForUserAsync(email, id);
            if (order is null) return NotFound(new ApiResponse(StatusCodes.Status404NotFound));
            return Ok(order);
        }
    }
}
