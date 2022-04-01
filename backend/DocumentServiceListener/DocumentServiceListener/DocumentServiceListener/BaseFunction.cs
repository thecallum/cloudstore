using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Util;
using AWSServerless1;
using AWSServerless1.Formatters;
using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Gateways.Interfaces;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Services;
using DocumentServiceListener.UseCase;
using DocumentServiceListener.UseCase.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using static Amazon.Lambda.SQSEvents.SQSEvent;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DocumentServiceListener
{
    public class LambdaEntryPoint
    {
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
            builder.AddJsonFile("appsettings.json");
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            if (!string.IsNullOrEmpty(environment))
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"appsettings.{environment}.json");
                if (File.Exists(path))
                    builder.AddJsonFile(path);
            }
            builder.AddEnvironmentVariables();
        }

        protected static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAmazonS3>(x => new AmazonS3Client());

            services.AddScoped<IDocumentUploadedUseCase, DocumentUploadedUseCase>();
            services.AddScoped<IDocumentDeletedUseCase, DocumentDeletedUseCase>();
            services.AddScoped<IDirectoryDeletedUseCase, DirectoryDeletedUseCase>();
            services.AddScoped<IAccountDeletedUseCase, AccountDeletedUseCase>();

            services.AddScoped<IS3Gateway, S3Gateway>();
            services.AddScoped<IDocumentGateway, DocumentGateway>();
            services.AddScoped<IDirectoryGateway, DirectoryGateway>();

            services.AddScoped<IStorageUsageCache, StorageUsageCache>();

            services.AddScoped<IImageFormatter, ImageFormatter>();
            services.AddScoped<IImageLoader, ImageLoader>();

            services.ConfigureRedis();

            ConfigureDbContext(services);
            services.ConfigureAws();
        }

        protected static void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? null;

            services.AddDbContext<DocumentServiceContext>(options =>
            {
                if (connectionString != null)
                {
                    options.UseNpgsql(connectionString);
                }
                else
                {
                    options.UseInMemoryDatabase("integration");
                    options.ConfigureWarnings(warningOptions =>
                    {
                        warningOptions.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                    });
                }
            });
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            // Do this in parallel???
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context).ConfigureAwait(false);
            }
        }

        private static CloudStoreSnsEvent DeserialiseSnsMessage(SQSMessage message)
        {
            var body = JsonSerializer.Deserialize<SnsBody>(message.Body, _jsonOptions);

            var snsEvent = JsonSerializer.Deserialize<CloudStoreSnsEvent>(body.Message, _jsonOptions);

            return snsEvent;
        }

        private async Task ProcessMessageAsync(SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogLine($"Processing message {message.MessageId}");
           
            var entityEvent = DeserialiseSnsMessage(message);

            var eventHandler = EventHandlerFactory.Find(entityEvent.EventName, ServiceProvider);
            if (eventHandler == null) throw new Exception($"EventName [{entityEvent.EventName}] not found");

            await eventHandler.ProcessMessageAsync(entityEvent);
        }

        private readonly static JsonSerializerOptions _jsonOptions = CreateJsonOptions();

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }


}