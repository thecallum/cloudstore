using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using authservice.Boundary.Request;
using authservice.Boundary.Request.Validation;
using authservice.Encryption;
using authservice.Gateways;
using authservice.JWT;
using authservice.UseCase;
using authservice.UseCase.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



namespace authservice
{
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

            services.AddMvc(setup => {
                //...mvc setup...
            }).AddFluentValidation();

            services.AddTransient<IValidator<LoginRequestObject>, LoginRequestObjectValidation>();
            services.AddTransient<IValidator<RegisterRequestObject>, RegisterRequestObjectValidation>();

            services.AddScoped<IUserGateway, UserGateway>();

            services.AddScoped<ILoginUseCase, LoginUseCase>();
            services.AddScoped<IRegisterUseCase, RegisterUseCase>();
            services.AddScoped<IDeleteUseCase, DeleteUseCase>();

            services.AddTransient<ITokenService>(x => new TokenService(Environment.GetEnvironmentVariable("SECRET")));
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            ConfigureDynamoDbAsync(services).GetAwaiter().GetResult();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
            });

        }

        private async Task ConfigureDynamoDbAsync(IServiceCollection services)
        {

            bool localMode = false;
            _ = bool.TryParse(Environment.GetEnvironmentVariable("DynamoDb_LocalMode"), out localMode);

            if (localMode)
            {
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = "http://localhost:8000" };
                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else
            {


                services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
                services.AddAWSService<IAmazonDynamoDB>();
            }

            services.AddScoped<IDynamoDBContext>(sp =>
            {
                var db = sp.GetService<IAmazonDynamoDB>();
                return new DynamoDBContext(db);
            });
        }
    }
}
