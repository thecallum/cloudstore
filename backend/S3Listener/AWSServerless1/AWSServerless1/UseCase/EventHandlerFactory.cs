using System;
using static Amazon.S3.Util.S3EventNotification;
using Microsoft.Extensions.DependencyInjection;

namespace AWSServerless1
{
    public static class EventHandlerFactory
    {
        public static IMessageProcessor Find(S3Entity entity, IServiceProvider serviceProvider)
        {
            switch (entity.ConfigurationId)
            {
                case "DocumentUploaded":
                    return serviceProvider.GetService<IDocumentUploadedUseCase>();
  
                case "DocumentDeleted":
                    return serviceProvider.GetService<IDocumentDeletedUseCase>();

                default:
                    return null;
            }
        }
    }
}
