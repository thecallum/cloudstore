using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using DocumentServiceListener.Boundary.Request;
using DocumentServiceListener.UseCase;
using DocumentServiceListener.UseCase.Interfaces;
using Newtonsoft.Json.Linq;
using static Amazon.Lambda.SQSEvents.SQSEvent;



// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DocumentServiceListener
{
    public class Function : BaseFunction
    {
        public Function()
        {
            // _deleteDirectoryUseCase = new DeleteDirectoryUseCase();
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
           // services.ConfigureDynamoDB();

          //  services.AddHttpClient();

            services.AddScoped<IDeleteDirectoryUseCase, DeleteDirectoryUseCase>();
          //  services.AddScoped<IUpdateAccountDetailsOnAssetTenure, UpdateAccountDetailsOnAssetTenure>();

            // Transient because otherwise all gateway's that use it will get the same instance,
            // which is not the desired result.
           // services.AddTransient<IApiGateway, ApiGateway>();

            base.ConfigureServices(services);
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            var taskList = new List<Task>();

            foreach(var message in evnt.Records)
            {
                taskList.Add(ProcessMessageAsync(message, context));
            }

            await Task.WhenAll(taskList);
        }

        private SnsEvent ParseEventObject(SQSMessage message, ILambdaContext context)
        {
            try
            {
                var jo = JObject.Parse(message.Body);
                var rawEvent = jo["Message"].ToString();

                var entityEvent = JsonSerializer.Deserialize<SnsEvent>(rawEvent, _jsonOptions);

                return entityEvent;
            } catch(Exception)
            {
                context.Logger.LogLine($"Invalid message format");

                throw;
            }
        }

        private async Task ProcessMessageAsync(SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogLine($"Processed message {message.Body}");

            var entityEvent = ParseEventObject(message, context);

            

            context.Logger.LogLine($"EventType {entityEvent.EventType}");


            try
            {
                IMessageProcessing processor = entityEvent.CreateUseCaseForMessage(ServiceProvider);


                if (processor == null)
                {
                    context.Logger.Log($@"No processors available for message so it will be ignored Message id: {message.MessageId}; type: {entityEvent.EventType}");

                    return;
                }


                await processor.ProcessMessageAsync(entityEvent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.Logger.Log($"Exception processing message id: {message.MessageId}; type: {entityEvent.EventType}");
                throw; // AWS will handle retry/moving to the dead letter queue
            }



           
        }
    }

    public interface IMessageProcessing
    {
        Task ProcessMessageAsync(SnsEvent message);
    }

    public static class UseCaseFactory
    {
        public static IMessageProcessing CreateUseCaseForMessage(this SnsEvent entityEvent, IServiceProvider serviceProvider)
        {
            if (entityEvent is null) throw new ArgumentNullException(nameof(entityEvent));
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            IMessageProcessing processor = null;
            switch (entityEvent.EventType)
            {
                case EventTypes.DeleteDirectoryEvent:
                    {
                        processor = serviceProvider.GetService<IDeleteDirectoryUseCase>();
                        break;
                    }

                default:
                    throw new ArgumentException($"Unknown event type: {entityEvent.EventType}");
            }

            return processor;
        }
    }
}
