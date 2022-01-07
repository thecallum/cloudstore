using System;
using static Amazon.S3.Util.S3EventNotification;
using Microsoft.Extensions.DependencyInjection;
using DocumentServiceListener.UseCase;
using DocumentServiceListener.UseCase.Interfaces;

namespace AWSServerless1
{
    public static class EventHandlerFactory
    {
        public static IMessageProcessor Find(string eventName, IServiceProvider serviceProvider)
        {
            switch (eventName)
            {
                case EventNames.DocumentUploaded:
                    return serviceProvider.GetService<IDocumentUploadedUseCase>();
  
                case EventNames.DocumentDeleted:
                    return serviceProvider.GetService<IDocumentDeletedUseCase>();

                case EventNames.DirectoryDeleted:
                    return serviceProvider.GetService<IDirectoryDeletedUseCase>();

                case EventNames.AccountDeleted:
                    return serviceProvider.GetService<IAccountDeletedUseCase>();

                default:
                    return null;
            }
        }
    }
}
