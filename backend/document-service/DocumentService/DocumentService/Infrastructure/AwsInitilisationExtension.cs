using System;
using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumentService.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class AwsInitilisationExtension
    {
        public static void ConfigureAws(this IServiceCollection services)
        {
            var localMode = false;
            _ = bool.TryParse(Environment.GetEnvironmentVariable("DynamoDb_LocalMode"), out localMode);

            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production";

            if (isDevelopment || localMode)
            {
                services.AddScoped<IAmazonS3>(x =>
                {
                    var config = new AmazonS3Config { ServiceURL = "http://localhost:4566", ForcePathStyle = true };
                    return new AmazonS3Client(config);
                });


                services.TryAddSingleton<IAmazonSimpleNotificationService>(sp =>
                {
                    var clientConfig = new AmazonSimpleNotificationServiceConfig {
                        ServiceURL = "http://localhost:4566"
                    };

                    return new AmazonSimpleNotificationServiceClient(clientConfig);
                });
            }
            else
            {
                services.AddScoped<IAmazonS3>(x => new AmazonS3Client());
                services.AddScoped<IAmazonSimpleNotificationService>(x => new AmazonSimpleNotificationServiceClient());
            }
        }
    }
}
