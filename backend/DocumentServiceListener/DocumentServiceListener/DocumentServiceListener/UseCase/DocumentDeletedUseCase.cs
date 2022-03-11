using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AWSServerless1
{
    public class DocumentDeletedUseCase : IDocumentDeletedUseCase
    {
        private readonly IS3Gateway _s3Gateway;

        public DocumentDeletedUseCase(IS3Gateway s3Gateway)
        {
            _s3Gateway = s3Gateway;
        }

        public async Task ProcessMessageAsync(CloudStoreSnsEvent entity)
        {
            Console.WriteLine($"Document Was DELETED");

            var objectId = EntityHelper.TryParseGuid(entity.Body, "DocumentId");
            if (objectId == null) throw new ArgumentNullException(nameof(entity));

            var documentKey = $"{entity.User.Id}/{objectId}";

            await _s3Gateway.DeleteThumbnail(documentKey);
        }
    }
}
