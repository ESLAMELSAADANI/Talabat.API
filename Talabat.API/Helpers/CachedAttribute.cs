using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.API.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }

        //Called before the endpoint executed after modelbinding completed.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            //Ask CLR For Creating object from "ResponseCacheService" Explicitly not implicitly by the CLR.

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var response = await responseCacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = result;
                return;
            }//

            var executedActionContext = await next.Invoke();//will execute the next action filter or the action itself if this filter is the last filter on the action/endpoint 

            if (executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds))
            }

        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            // {{url}}/api/products?pageIndex=1&pageSize=5&sort=name

            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path);// /api/products

            //pageIndex=1&pageSize=5&sort=name
            foreach (var (key, value) in request.Query)
            {
                keyBuilder.Append($"|{key}-{value}");
                //1st iteration => /api/products|pageIndex-1
                //2nd iteration => /api/products|pageIndex-1|pageSize-5
                //3rd iteration => /api/products|pageIndex-1|pageSize-5|sort-name
            }

            return keyBuilder.ToString();
        }
    }
}
