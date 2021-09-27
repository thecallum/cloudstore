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
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = Environment.GetEnvironmentVariable("Localstack_URL") };
                    return new AmazonDynamoDBClient(clientConfig);
                });

                services.AddScoped<IAmazonS3>(x =>
                {
                    var config = new AmazonS3Config { ServiceURL = Environment.GetEnvironmentVariable("Localstack_URL"), ForcePathStyle = true };
                    return new AmazonS3Client(config);
                });
            }
            else
            {
                services.TryAddAWSService<IAmazonDynamoDB>();
                services.TryAddAWSService<IAmazonS3>();
            }

            services.AddScoped<IDynamoDBContext>(sp =>
            {
                var db = sp.GetService<IAmazonDynamoDB>();
                return new DynamoDBContext(db);
            });

            
        }
    }
}
