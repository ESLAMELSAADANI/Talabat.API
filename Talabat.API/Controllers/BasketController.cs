using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Repositories.Contract;

namespace Talabat.API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepository;

        public BasketController(IMapper mapper, IBasketRepository basketRepository)
        {
            _mapper = mapper;
            _basketRepository = basketRepository;
        }

        [HttpGet]//Get: /api/basket?id={id}
        [EndpointSummary("Get basket by it's id")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return Ok(basket is null ? new CustomerBasket(id) : basket);
        }

        [HttpPost]//Post : /api/basket
        [EndpointSummary("create or update specific basket")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDTO basketDTO)
        {
            var basket = _mapper.Map<CustomerBasket>(basketDTO);
            var createdOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(basket);
            if (createdOrUpdatedBasket is null)
                return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest));
            return Ok(createdOrUpdatedBasket);
        }

        [HttpPut("{basketId}/delivery")]// POST : /api/{basketId}/delivery
        [EndpointSummary("Attach Basket To Delivery Method")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerBasket>> AttachBasketToDeliveryMethod(string basketId, SetDeliveryMethodDTO dto)
        {
            var basket = await _basketRepository.AttachBasketToDeliveryMethodAsync(basketId, dto.DeliveryMethodId);
            if (basket is null) return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest, "Invalid basket or delivery method"));
            return Ok(basket);
        }

        [HttpDelete]//Delete : /api/basket
        [EndpointSummary("Delete basket by it's id")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var basketDeleted = await _basketRepository.DeleteBasketAsync(id);
            return basketDeleted ?
                Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Basket Deleted!"
                }) :
            BadRequest(new
            {
                statusCode = StatusCodes.Status400BadRequest,
                message = "Failed To Delete!"
            }
            );
        }
    }
}
