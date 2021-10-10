using System;
using System.Diagnostics.CodeAnalysis;
using authservice.Boundary.Request;
using authservice.Boundary.Request.Validation;
using authservice.Encryption;
using authservice.Gateways;
using authservice.Infrastructure;
using authservice.UseCase;
using authservice.UseCase.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TokenService;

namespace authservice
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMvc(setup =>
            {
                //...mvc setup...
            }).AddFluentValidation();

            RegisterValidators(services);


            services.ConfigureAws();

            RegisterGateways(services);
            RegisterUseCases(services);

            services.AddTransient<ITokenService>(x => new TokenService.TokenService(Environment.GetEnvironmentVariable("SECRET")));
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            services.AddTransient<IValidator<LoginRequestObject>, LoginRequestObjectValidation>();
            services.AddTransient<IValidator<RegisterRequestObject>, RegisterRequestObjectValidation>();
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IUserGateway, UserGateway>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddScoped<ILoginUseCase, LoginUseCase>();
            services.AddScoped<IRegisterUseCase, RegisterUseCase>();
            services.AddScoped<IDeleteUseCase, DeleteUseCase>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Map("/auth-service", mainApp =>
            {
                if (env.IsDevelopment()) mainApp.UseDeveloperExceptionPage();

                mainApp.UseHttpsRedirection();

                mainApp.UseRouting();

                mainApp.UseAuthorization();

                mainApp.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGet("/",
                        async context =>
                        {
                            await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                        });
                });
            });
        }
    }
}