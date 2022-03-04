using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public class SnsGateway : ISnsGateway
    {
        private readonly IAmazonSimpleNotificationService _SNSService;
        private readonly static JsonSerializerOptions _jsonOptions = CreateJsonOptions();
        private const string MessageGroupId = "DocumentService";

        private readonly string SnsTopicArn;

        public SnsGateway(IAmazonSimpleNotificationService snsService)
        {
            _SNSService = snsService;

            SnsTopicArn = Environment.GetEnvironmentVariable("SNS_TOPIC_ARN");
            // "arn:aws:sns:eu-west-1:714664911966:DocumentService.fifo"
        }

        public async Task PublishDocumentUploadedEvent(User user, Guid documentId)
        {
            // build event

            Console.WriteLine($"Calling SNSGateway for [user: {user?.Id}] [document: {documentId}]");

            var CSEvent = new CloudStoreSnsEvent()
            {
                EventName = EventNames.DocumentUploaded,
                User = user,
                Body = new Dictionary<string, object>()
                {
                    { "DocumentId", documentId.ToString() }
                }
            };

            var msgBody = JsonSerializer.Serialize(CSEvent, _jsonOptions);
            var request = new PublishRequest
            {
                Message = msgBody,
                TopicArn = SnsTopicArn,
                MessageGroupId = MessageGroupId
            };

            // publish event

            Console.WriteLine("Publishing DocumentUploadedEvent");

            await _SNSService.PublishAsync(request);
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
