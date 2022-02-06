using System;
using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumentService.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class AwsInitilisationExtension
    {
        private const string TestTopicName = "TESTTOPIC";

        public static void ConfigureAws(this IServiceCollection services)
        {
            var localMode = false;
            _ = bool.TryParse(Environment.GetEnvironmentVariable("DynamoDb_LocalMode"), out localMode);

            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production";

            if (isDevelopment || localMode)
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

                services.AddScoped<IAmazonSimpleNotificationService>(x =>
                {
                    var clientConfig = new AmazonSimpleNotificationServiceConfig
                    {
                        ServiceURL = localstackUrl ?? "http://localhost:4566"
                    };

                    var client = new AmazonSimpleNotificationServiceClient(clientConfig);

                    var topicArn = CreateTestTopic(client);
                    Environment.SetEnvironmentVariable("SNS_TOPIC_ARN", topicArn);

                    return client;
                });
            }
            else
            {
                services.AddScoped<IAmazonS3>(x => new AmazonS3Client());
                services.AddScoped<IAmazonSimpleNotificationService>(x => new AmazonSimpleNotificationServiceClient());
            }
        }

        private static string CreateTestTopic(AmazonSimpleNotificationServiceClient client)
        {
            var request = new CreateTopicRequest { Name = TestTopicName };
            var response = client.CreateTopicAsync(request).GetAwaiter().GetResult();

            return response.TopicArn;
        }
    }
}
