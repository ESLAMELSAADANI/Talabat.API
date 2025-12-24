using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;

namespace Talabat.API.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService,
            IConfiguration configuration,
            ILogger<PaymentController> logger
            )
        {
            _paymentService = paymentService;
            _configuration = configuration;
            _logger = logger;
        }
        [Authorize]
        [HttpPost("{basketId}")]// POST : /api/payment/{basketId}
        [EndpointSummary("Create or update payment intent")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);

            if (basket is null) return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest, "An Error With Your Basket"));
            return Ok(basket);
        }
        [HttpPost("webhook")]
        //This endpoint will consumed by stripe and send to it the state of orderEvent [success - failed]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _configuration["StripeSettings:WebhookSecret"]);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            //Handle the event
            Order? order;

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, true);
                    _logger.LogInformation("Order is Succeeded, paymentIntentId: {0}", order?.PaymentIntentId);
                    _logger.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
                    break;
                case EventTypes.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, false);
                    _logger.LogInformation("Order is Failed, paymentIntentId: {0}", order?.PaymentIntentId);
                    _logger.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
                    break;
                default:
                    break;
            }
            return Ok();

        }

    }
}
