using StackExchange.Redis;
using System.Text.Json;
using Talabat.Core;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;

namespace Talabat.Infrastructure.Basket_Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        private readonly IUnitOfWork _unitOfWork;

        public BasketRepository(
            IConnectionMultiplexer redis,
            IUnitOfWork unitOfWork
            )
        {
            _database = redis.GetDatabase();
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var basket = await _database.StringGetAsync(basketId);//Get Basket
            return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }
        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var createdOrUpdated = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));
            if (!createdOrUpdated)
                return null;
            return await GetBasketAsync(basket.Id);
        }
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket?> AttachBasketToDeliveryMethodAsync(string basketId, int deliveryMethodId)
        {
            var redisBasket = await _database.StringGetAsync(basketId);
            if (!redisBasket.HasValue) return null;

            var basket = JsonSerializer.Deserialize<CustomerBasket>(redisBasket);

            basket.DeliveryMethodId = deliveryMethodId;

            await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));

            return basket;

        }
    }
}
