using System;
using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    // var clientConfig = new AmazonDynamoDBConfig { ServiceURL = Environment.GetEnvironmentVariable("Localstack_URL") };
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = "http://localhost:4566" };
                    return new AmazonDynamoDBClient(clientConfig);
                });

                services.AddScoped<IAmazonS3>(x =>
                {
                    //  var config = new AmazonS3Config { ServiceURL = Environment.GetEnvironmentVariable("Localstack_URL"), ForcePathStyle = true };
                    var config = new AmazonS3Config { ServiceURL = "http://localhost:4566", ForcePathStyle = true };
                    return new AmazonS3Client(config);
                });
            }
            else
            {
                services.AddSingleton<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient());
                services.AddScoped<IAmazonS3>(x => new AmazonS3Client());
            }

            services.AddScoped<IDynamoDBContext>(sp =>
            {
                var db = sp.GetService<IAmazonDynamoDB>();
                return new DynamoDBContext(db);
            });

            
        }
    }
}
