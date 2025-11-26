namespace Talabat.API.Extensions
{
    public static class SwaggerMiddlewaresExtensions
    {
        public static WebApplication UseSwaggerMiddlewares(this WebApplication app)
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });

            return app;
        }
    }
}
