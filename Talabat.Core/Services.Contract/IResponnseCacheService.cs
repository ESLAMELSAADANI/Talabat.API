using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Services.Contract
{
    public interface IResponnseCacheService
    {
        Task CacheResponseAsync(string key, object response,TimeSpan timeToLive);

        Task<string?> GetCachedResponseAsync(string key); 
    }
}
