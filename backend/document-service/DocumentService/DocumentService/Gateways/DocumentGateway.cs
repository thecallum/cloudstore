using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.Gateways
{
    public class DocumentGateway : IDocumentGateway
    {
        private readonly DocumentServiceContext _documentStorageContext;

        public DocumentGateway(DocumentServiceContext documentStorageContext)
        {
            _documentStorageContext = documentStorageContext;
        }

        public async Task<DocumentDomain> DeleteDocument(Guid userId, Guid documentId)
        {
            LogHelper.LogGateway("DocumentGateway", "DeleteDocument");

            var existingDocument = await LoadDocument(userId, documentId);
            if (existingDocument == null) throw new DocumentNotFoundException();

            _documentStorageContext.Documents.Remove(existingDocument);

            await _documentStorageContext.SaveChangesAsync();

            return existingDocument.ToDomain();
        }

        public async Task<bool> DirectoryContainsFiles(Guid userId, Guid directoryId)
        {
            LogHelper.LogGateway("DocumentGateway", "DirectoryContainsFiles");

            var documents = await GetAllDocuments(userId, directoryId);

            return documents.Count() > 0;
        }

        public async Task<IEnumerable<DocumentDomain>> GetAllDocuments(Guid userId, Guid? directoryId = null)
        {
            LogHelper.LogGateway("DocumentGateway", "GetAllDocuments");

            var documents = await _documentStorageContext.Documents
                .Where(x => x.UserId == userId && x.DirectoryId == directoryId)
                .ToListAsync();

            return documents.Select(x => x.ToDomain());
        }

        public async Task<DocumentDomain> GetDocumentById(Guid userId, Guid documentId)
        {
            LogHelper.LogGateway("DocumentGateway", "GetDocumentById");

            var document = await LoadDocument(userId, documentId);

            return document?.ToDomain();
        }

        private async Task<DocumentDb> LoadDocument(Guid userId, Guid documentId)
        {
            LogHelper.LogGateway("DocumentGateway", "GetDocumentById");

            var document = await _documentStorageContext.Documents
                .Where(x => x.UserId == userId && x.Id == documentId)
                .SingleOrDefaultAsync();

            return document;
        }

        public async Task SaveDocument(DocumentDomain document)
        {
            LogHelper.LogGateway("DocumentGateway", "SaveDocument");

            _documentStorageContext.Documents.Add(document.ToDatabase());

            await _documentStorageContext.SaveChangesAsync();
        }

        public async Task UpdateDocument(DocumentDomain document)
        {
            LogHelper.LogGateway("DocumentGateway", "UpdateDocument");

            var existingDocument = await LoadDocument(document.UserId, document.Id);

            // only filesize can be changed
            existingDocument.FileSize = document.FileSize;

            await _documentStorageContext.SaveChangesAsync();
        }

        public async Task<StorageUsageResponse> GetUsage(User user)
        {
            var total = await LoadStorageUsage(user.Id);

            return new StorageUsageResponse
            {
                StorageUsage = total,
                Capacity = user.StorageCapacity
            };
        }

        public async Task<bool> CanUploadFile(User user, long fileSize, long? originalFileSize = null)
        {
            var totalUsage = await LoadStorageUsage(user.Id);

            var expectedUsage = totalUsage + fileSize;
            if (originalFileSize != null) expectedUsage -= (long)originalFileSize;
            return expectedUsage < user.StorageCapacity;
        }
        private async Task<long> LoadStorageUsage(Guid userId)
        {
            var total = await _documentStorageContext.Documents
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.FileSize);

            return total;
        }
    }
}
