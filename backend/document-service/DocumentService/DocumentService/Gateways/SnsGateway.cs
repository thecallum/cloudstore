using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.Gateways
{
    public class SnsGateway : ISnsGateway
    {
        private readonly IAmazonSimpleNotificationService _SNSService;
        private readonly static JsonSerializerOptions _jsonOptions = CreateJsonOptions();
        private const string MessageGroupId = "DocumentService";

        public SnsGateway(IAmazonSimpleNotificationService snsService)
        {
            _SNSService = snsService;
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
                TopicArn = "arn:aws:sns:eu-west-1:714664911966:DocumentService.fifo",
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
