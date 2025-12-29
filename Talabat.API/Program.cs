using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using Talabat.API.Extensions;
using Talabat.API.Middlewares;
using Talabat.Application.AuthService;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Infrastructure._Identity;
using Talabat.Infrastructure.Basket_Repository;
using Talabat.Infrastructure.Data;

namespace Talabat.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            #region Add Services To DIC - Dependency Injection Container

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//ignore reference loop when serialize, just serialize one level of the object - Navigationprop
            }
            );
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            //builder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
            //builder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //======== Instead of previous lines =======
            //ApplicationServicesExtension.AddApplicationServices(builder.Services);
            builder.Services.AddApplicationServices();//Call it as extension method for the contaier builder.services which is of type IServiceCollection

            //Add Redis Service To DIC
            builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection!);
            });

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();

            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });


            //Add Authentication Services
            builder.Services.AddAuthServices(builder);

            //Register CORS Services with policies [من أي مصدر خارجي  api سياسات التعامل مع بروجكت ال] 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policyOptions =>
                {
                    policyOptions.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            }
                );

            #endregion

            var app = builder.Build();

            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });

            //Ask CLR For Creating Object From DbContext (StoreContext) Explicitly
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();
            var _IdentityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();


            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger<Program>();
            try
            {
                await _dbContext.Database.MigrateAsync();//Update-Database => For StoreContext
                //await StoreContextSeed.SeedAsync(_dbContext);//Data Seeding => For StoreContext
                await StoreContextSeed.SeedAsync(_dbContext);//Data Seeding => For StoreContext

                await _IdentityDbContext.Database.MigrateAsync();//update-Database => For ApplicationIdentityDbContext
                var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                await ApplicationIdentityDbContextSeed.SeedUserAsync(_userManager);//DataSeeding => For ApplicationIdentityDbContext
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);//Kestrel screen
                logger.LogError(ex, "An error has been occurred when apply the migration");
            }


            #region Configure Middlewares

            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionMiddleware>();

            ///Make Middleware By Request Delegate approach in program.cs instead of make middleware class.
            ///app.Use(async (httpContext, _next) =>
            ///{
            ///    try
            ///    {
            ///        await _next.Invoke(httpContext);
            ///    }
            ///    catch (Exception ex)
            ///    {
            ///
            ///        logger.LogError(ex.Message);
            ///
            ///        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ///        httpContext.Response.ContentType = "application/json";
            ///
            ///        var response = app.Environment
            ///                          .IsDevelopment() ?
            ///                          new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) :
            ///                          new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
            ///        await httpContext.Response.WriteAsJsonAsync(response);
            ///
            ///    }
            ///});

            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                //app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
                app.UseSwaggerMiddlewares();//Extension Method.
            }

            //app.UseStatusCodePagesWithRedirects("/Errors/{0}");//Work When Send Request With Invalid URL - To Redirect this request to specific route/URL/endpoint
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");//I use this, bcz the previous method make redirection when type invalid URL, and redirect me on another URL, but i use response return to me without redirection , in the same URL.
            //This Parameter To Check The Type of error that trigger this middleware
            //if (Unauthorize URL) Or (Invalid URl)
            //If not provide URL => All responses will ne NotFound()
            //And this is not logic bcz if unauthorize URL need to return UnAuthorized() not NotFound()
            //In Contrloller i will bind this parameter to the status code of the error.
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();


            app.Run();

            #endregion

        }
    }
}
