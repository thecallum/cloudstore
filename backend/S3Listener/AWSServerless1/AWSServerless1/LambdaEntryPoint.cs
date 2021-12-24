using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Util;
using AWSServerless1.Formatters;
using AWSServerless1.Gateways;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSServerless1
{
    public class LambdaEntryPoint {

        protected IConfigurationRoot Configuration { get; }
        protected IServiceProvider ServiceProvider { get; }

        public LambdaEntryPoint()
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder();

            Configure(builder);
            Configuration = builder.Build();
            services.AddSingleton<IConfiguration>(Configuration);

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected virtual void Configure(IConfigurationBuilder builder)
        {
            //builder.AddJsonFile("appsettings.json");
            //var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            //if (!string.IsNullOrEmpty(environment))
            //{
            //    var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"appsettings.{environment}.json");
            //    if (File.Exists(path))
            //        builder.AddJsonFile(path);
            //}
            //builder.AddEnvironmentVariables();
        }

        protected  void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAmazonS3>(x => new AmazonS3Client());

            services.AddScoped<IDocumentUploadedUseCase, DocumentUploadedUseCase>();
            services.AddScoped<IDocumentDeletedUseCase, DocumentDeletedUseCase>();

            services.AddScoped<IS3Gateway, S3Gateway>();
            services.AddScoped<IImageFormatter, ImageFormatter>();

        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            // Do this in parallel???
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context).ConfigureAwait(false);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogLine($"Processing message {message.MessageId}");

            var entityEvent = S3EventNotification.ParseJson(message.Body);
            var eventContent = entityEvent.Records[0].S3;

            var eventHandler = EventHandlerFactory.Find(eventContent, ServiceProvider);

            if (eventHandler == null)
            {
                throw new Exception($"EventName [{eventContent.ConfigurationId}] not found");
            }

            await eventHandler.ProcessMessageAsync(eventContent);
        }
    }
}
