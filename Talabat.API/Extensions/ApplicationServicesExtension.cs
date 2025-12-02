using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.API.Middlewares;
using Talabat.Core.Repositories.Contract;
using Talabat.Infrastructure.Generic_Repository;

namespace Talabat.API.Extensions
{
    public static class ApplicationServicesExtension
    {
        //This Method Add Services To the container DIC that is of type IServiceCollection.
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            services.AddAutoMapper(M => M.AddProfile(typeof(MappingProfiles)));

            services.AddTransient<ProductPictureUrlResolver>();

            services.Configure<ApiBehaviorOptions>(options =>
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

            services.AddTransient<ExceptionMiddleware>();

            return services;

        }
    }
}
