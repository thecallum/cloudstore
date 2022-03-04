using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
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

            var objectId = TryParseGuid(entity.Body, "DocumentId");
            if (objectId == null) throw new ArgumentNullException(nameof(entity));

            var documentKey = $"{entity.User.Id}/{objectId}";

            await _s3Gateway.DeleteThumbnail(documentKey);
        }

        private static Guid? TryParseGuid(Dictionary<string, object> body, string name)
        {
            if (!body.ContainsKey(name)) return null;

            var paramter = body[name].ToString();

            try
            {
                return Guid.Parse(paramter);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
