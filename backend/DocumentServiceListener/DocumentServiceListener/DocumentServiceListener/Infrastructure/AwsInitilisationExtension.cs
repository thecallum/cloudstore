using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DocumentServiceListener.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class AwsInitilisationExtension
    {
        public static void ConfigureAws(this IServiceCollection services)
        {
            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production";

            if (isDevelopment || false)
            {
                var localstackUrl = Environment.GetEnvironmentVariable("Localstack_url");

                services.AddScoped<IAmazonS3>(x =>
                {
                    var config = new AmazonS3Config { 
                        ServiceURL = localstackUrl ?? "http://localhost:4566", 
                        ForcePathStyle = true 
                    };

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
