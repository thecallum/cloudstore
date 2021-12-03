using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;




namespace DocumentServiceListener
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseFunction
    {
        protected readonly static JsonSerializerOptions _jsonOptions = CreateJsonOptions();

        protected IConfigurationRoot Configuration { get; }
        protected IServiceProvider ServiceProvider { get; }

        internal BaseFunction()
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder();

            Configure(builder);
            Configuration = builder.Build();
            services.AddSingleton<IConfiguration>(Configuration);

            ConfigureServices(services);

            // TODO - Remove if not using DynamoDb
           // if (Configuration.GetValue<bool>("DynamoDb_LocalMode"))
                //AWSXRayRecorder.Instance.ContextMissingStrategy = ContextMissingStrategy.LOG_ERROR;

            ServiceProvider = services.BuildServiceProvider();
            //ServiceProvider.UseLogCall();

         //   Logger = ServiceProvider.GetRequiredService<ILogger<BaseFunction>>();
        }

        protected static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
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

        protected virtual void ConfigureServices(IServiceCollection services)
        {
           // services.AddLogCallAspect();
        }
    }
}