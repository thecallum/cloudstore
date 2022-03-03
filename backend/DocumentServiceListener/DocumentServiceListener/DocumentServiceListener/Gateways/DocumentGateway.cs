using DocumentServiceListener.Gateways.Exceptions;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public class DocumentGateway : IDocumentGateway
    {
        private readonly DocumentServiceContext _documentStorageContext;
        private readonly string _s3BaseThumbnailPath = Environment.GetEnvironmentVariable("S3_BUCKET_BASE_PATH");

        public DocumentGateway(DocumentServiceContext documentStorageContext)
        {
            _documentStorageContext = documentStorageContext;
        }

        public async Task UpdateThumbnail(Guid userId, Guid documentId)
        {
            LogHelper.LogGateway("DocumentGateway", "UpdateThumbnail");

            var document = await LoadDocument(userId, documentId);
            if (document == null) throw new DocumentDbNotFoundException(userId, documentId);
            
            var newPath = $"{_s3BaseThumbnailPath}/{userId}/{documentId}";

            Console.WriteLine($"Saving Thumbnail: {newPath}");

            document.Thumbnail = newPath;

            await _documentStorageContext.SaveChangesAsync();
        }

        private async Task<DocumentDb> LoadDocument(Guid userId, Guid documentId)
        {
            LogHelper.LogGateway("DocumentGateway", "LoadDocument");

            var document = await _documentStorageContext.Documents.FindAsync(documentId);
            if (document == null) return null;

            if (document.UserId != userId) {
                Console.WriteLine("Document.UserId doesn't match");
                return null;
            }

            return document;
        }
    }
}
