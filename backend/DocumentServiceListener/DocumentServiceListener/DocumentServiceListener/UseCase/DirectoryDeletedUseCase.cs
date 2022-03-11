using Amazon.S3.Util;
using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Gateways.Interfaces;
using DocumentServiceListener.Helpers;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.UseCase
{
    public class DirectoryDeletedUseCase : IDirectoryDeletedUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;
        private readonly IDirectoryGateway _directoryGateway;

        public DirectoryDeletedUseCase(IS3Gateway s3Gateway, IDocumentGateway documentGateway, IDirectoryGateway directoryGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
            _directoryGateway = directoryGateway;
        }

        public async Task ProcessMessageAsync(CloudStoreSnsEvent entity)
        {
            var directoryId = EntityHelper.TryParseGuid(entity.Body, "DirectoryId");
            if (directoryId == null) throw new ArgumentNullException(nameof(entity));

            // 1. Get list of all Documents in directory
            var documents = await _documentGateway.GetAllDocuments(entity.User.Id, (Guid) directoryId);

            if (documents.Any())
            {
                // 2. Call DeleteObjectsRequest for all objects in /store
                await DeleteDocumentsFromS3(documents);
            
                // 3. Call DeleteObjectsRequest for all thumbnails
                await DeleteThumbnailsFromS3(documents);

                // 4. Delete all documents in DB where directoryId is X
                await _documentGateway.DeleteAllDocuments(entity.User.Id, (Guid) directoryId);
            }

            // 5. Delete Directory from DB
            await _directoryGateway.DeleteDirectory(entity.User.Id, (Guid) directoryId);
        }

        private async Task DeleteDocumentsFromS3(IEnumerable<DocumentDb> documents)
        {
            var documentKeys = GetObjectKeys(documents, "store");
            await _s3Gateway.DeleteDocuments(documentKeys);
        }

        private async Task DeleteThumbnailsFromS3(IEnumerable<DocumentDb> documents)
        {
            var thumbnailKeys = GetObjectKeys(documents, "thumbnails");
            await _s3Gateway.DeleteDocuments(thumbnailKeys);
        }

        private static List<string> GetObjectKeys(IEnumerable<DocumentDb> documents, string pathName)
        {
            return documents
                .Select(x => $"{pathName}/{x.UserId}/{x.Id}")
                .ToList();
        }
    }
}
