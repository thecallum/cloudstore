using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Request.Validation;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.UseCase;
using DocumentService.UseCase.Interfaces;
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

namespace DocumentService
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
            services.AddMvc(setup => {
                //...mvc setup...
            }).AddFluentValidation();

            services.AddTransient<IValidator<CreateDirectoryRequest>, CreateDirectoryRequestValidator>();
            services.AddTransient<IValidator<RenameDirectoryRequest>, RenameDirectoryRequestValidator>();

            services.AddControllers();

            services.ConfigureAws();

            SetupGateways(services);
            SetupUseCases(services);
        }

        private void SetupGateways(IServiceCollection services)
        {
            services.AddScoped<IDocumentGateway, DocumentGateway>();
            services.AddScoped<IDirectoryGateway, DirectoryGateway>();
            services.AddScoped<IS3Gateway, S3Gateway>();
        }

        private void SetupUseCases(IServiceCollection services)
        {
            services.AddScoped<IGetAllDocumentsUseCase, GetAllDocumentsUseCase>();
            services.AddScoped<ICreateDirectoryUseCase, CreateDirectoryUseCase>();
            services.AddScoped<IRenameDirectoryUseCase, RenameDirectoryUseCase>();
            services.AddScoped<IDeleteDirectoryUseCase, DeleteDirectoryUseCase>();
            services.AddScoped<IGetDocumentDownloadLinkUseCase, GetDocumentDownloadLinkUseCase>();
            services.AddScoped<IDeleteDocumentUseCase, DeleteDocumentUseCase>();
            services.AddScoped<IGetDocumentUploadLinkUseCase, GetDocumentUploadLinkUseCase>();
            services.AddScoped<IValidateUploadedDocumentUseCase, ValidateUploadedDocumentUseCase>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Map("/document-service", mainApp =>
            {
                if (env.IsDevelopment())
                {
                    mainApp.UseDeveloperExceptionPage();
                }

                mainApp.UseHttpsRedirection();

                mainApp.UseRouting();

                mainApp.UseAuthorization();


                mainApp.UseEndpoints(endpoints =>
                {

                    endpoints.MapControllers();
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                    });
                });
            });
        }
    }
}
