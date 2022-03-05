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
        }

        public async Task PublishDocumentUploadedEvent(User user, Guid documentId)
        {
            Console.WriteLine("Publishing DocumentUploadedEvent");

            var body = new Dictionary<string, object>()
            {
                { "DocumentId", documentId.ToString() }
            };

            await PublishEvent(user, body, EventNames.DocumentUploaded);
        }

        public async Task PublishDocumentDeletedEvent(User user, Guid documentId)
        {
            Console.WriteLine("Publishing DocumentDeletedEvent");

            var body = new Dictionary<string, object>()
            {
                { "DocumentId", documentId.ToString() }
            };

            await PublishEvent(user, body, EventNames.DocumentDeleted);
        }

        public async Task PublishDeleteDirectoryEvent(User user, Guid directoryId)
        {
            Console.WriteLine("Publishing DeleteDirectoryEvent");

            var body = new Dictionary<string, object>()
            {
                { "DirectoryId", directoryId.ToString() }
            };

            await PublishEvent(user, body, EventNames.DirectoryDeleted);
        }

        private async Task PublishEvent(User user, Dictionary<string, object> body, string eventName)
        {
            var CSEvent = new CloudStoreSnsEvent()
            {
                EventName = eventName,
                User = user,
                Body = body
            };

            var msgBody = JsonSerializer.Serialize(CSEvent, _jsonOptions);
            var request = new PublishRequest
            {
                Message = msgBody,
                TopicArn = SnsTopicArn,
                MessageGroupId = MessageGroupId
            };

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
