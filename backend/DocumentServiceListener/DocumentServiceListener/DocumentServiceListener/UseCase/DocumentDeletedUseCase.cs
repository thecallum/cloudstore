using DocumentServiceListener.Boundary;
using System;
using System.Threading.Tasks;
using static Amazon.S3.Util.S3EventNotification;

namespace AWSServerless1
{
    public class DocumentDeletedUseCase : IDocumentDeletedUseCase
    {
        public async Task ProcessMessageAsync(CloudStoreSnsEvent entity)
        {
            Console.WriteLine($"Document Was DELETED");
        }
    }
}
