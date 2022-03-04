using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways.Interfaces
{
    public interface IDocumentGateway
    {
        Task SaveDocument(DocumentDomain document);
        Task<DocumentDomain> GetDocumentById(Guid userId, Guid documentId);
        Task<IEnumerable<DocumentDomain>> GetAllDocuments(Guid userId, Guid? directoryId = null);
        Task<bool> DirectoryContainsFiles(Guid userId, Guid directoryId);
        Task<DocumentDomain> DeleteDocument(Guid userId, Guid documentId);
        Task<StorageUsageDomain> GetUsage(User user);
        Task<bool> CanUploadFile(User user, long fileSize, long? originalFileSize = null);
        Task UpdateDocument(DocumentDomain document);
    }
}