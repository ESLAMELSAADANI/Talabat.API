using System.Net;
using System.Text.Json;
using Talabat.API.Errors;

namespace Talabat.API.Middlewares
{
    //Make middleware by vonvension
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //You can write any code here to take action on the request

                await _next.Invoke(httpContext);//Go to the next middleware

                //You can write any code here to take action on the response returned
            }
            catch (Exception ex)
            {
                //1- Log the exception
                _logger.LogError(ex.Message);//If Development Env - Log Exception in (Database | Files) => If production env || So Support team can solve it later

                //2- Return Response to consumer of the endpoint | Response is formed in Header and body
                //- header => statusCode & Message & ContentType
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;//500 Internal server error
                httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment()
                                   ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                                   : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

                //var json = JsonSerializer.Serialize(response);
                await httpContext.Response.WriteAsJsonAsync(response);
                //- Body => 

            }
        }
    }
}
