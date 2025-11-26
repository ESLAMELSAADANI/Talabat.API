using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Infrastructure;
using Talabat.Infrastructure.Data;
using AutoMapper;
using Talabat.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Talabat.API.Middlewares;
using System.Net;

namespace Talabat.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            //builder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
            //builder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

            //======== Instead of previous lines =======
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddAutoMapper(M => M.AddProfile(typeof(MappingProfiles)));

            builder.Services.AddTransient<ProductPictureUrlResolver>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionResult) =>
                {
                    var errors = actionResult.ModelState.Where(parameter => parameter.Value.Errors.Count() > 0)
                                                        .SelectMany(parameter => parameter.Value.Errors)
                                                        .Select(error => error.ErrorMessage)
                                                        .ToList();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);
                };
            });

            var app = builder.Build();

            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });

            //Ask CLR For Creating Object From DbContext (StoreContext) Explicitly
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();


            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger<Program>();
            try
            {
                await _dbContext.Database.MigrateAsync();//Update-Database
                //await StoreContextSeed.SeedAsync(_dbContext);//Data Seeding
                await StoreContextSeed.SeedAsync(_dbContext);//Data Seeding
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);//Kestrel screen
                logger.LogError(ex, "An error has been occurred when apply the migration");
            }


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
                app.MapOpenApi();
                app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}
