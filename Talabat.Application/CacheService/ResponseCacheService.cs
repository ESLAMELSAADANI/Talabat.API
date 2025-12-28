using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Application.CacheService
{
    public class ResponseCacheService : IResponnseCacheService
    {
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string key, object response, TimeSpan timeToLive)
        {
            if (response is null) return;

            var serializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };//When Serialize to json it returned not as camel case and frontend only understand camel case so convert it first
            var serializedResponse = JsonSerializer.Serialize(response,serializeOptions);

            await _database.StringSetAsync(key, serializedResponse, timeToLive);
        }

        public async Task<string?> GetCachedResponseAsync(string key)
        {
            var response = await _database.StringGetAsync(key);

            if (response.IsNullOrEmpty) return null;
            return response;
        }
    }
}
