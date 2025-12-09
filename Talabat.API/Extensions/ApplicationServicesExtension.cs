using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.API.Middlewares;
using Talabat.Application.AuthService;
using Talabat.Core;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Infrastructure.Generic_Repository;
using Talabat.Infrastructure.UOW;

namespace Talabat.API.Extensions
{
    public static class ApplicationServicesExtension
    {
        //This Method Add Services To the container DIC that is of type IServiceCollection.
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Register Unit Of Work Sevice
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            
            //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


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

        public static IServiceCollection AddAuthServices(this IServiceCollection services,WebApplicationBuilder builder)
        {
            var jwtConfig = builder.Configuration.GetSection("JWT");
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//For Bearer Authentication Scheme That Validate the token.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//Specify the authentication schema 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//Specify Default challenge authentication schema validtor handler for all endpoints that use [Authorize] attribute, instead of write =>  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtConfig["ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtConfig["ValidAudience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["AuthKey"] ?? string.Empty)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                }
                       );
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
