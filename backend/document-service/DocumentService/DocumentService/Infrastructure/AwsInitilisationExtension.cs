using System;
using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentService.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class AwsInitilisationExtension
    {
        public static void ConfigureAws(this IServiceCollection services)
        {
            var localMode = false;
            _ = bool.TryParse(Environment.GetEnvironmentVariable("DynamoDb_LocalMode"), out localMode);

            if (localMode)
            {
                services.AddScoped<IAmazonS3>(x =>
                {
                    var config = new AmazonS3Config { ServiceURL = "http://localhost:4566", ForcePathStyle = true };
                    return new AmazonS3Client(config);
                });
            }
            else
            {
                services.AddScoped<IAmazonS3>(x => new AmazonS3Client());
            }
        }
    }
}
