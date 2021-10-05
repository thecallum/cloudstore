using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public interface IDocumentGateway
    {
        Task SaveDocument(Document document);
        Task<DocumentDb> GetDocumentById(Guid userId, Guid documentId);
        Task<IEnumerable<DocumentDb>> GetAllDocuments(Guid userId, Guid? directoryId = null);
        Task<bool> DirectoryContainsFiles(Guid userId, Guid directoryId);
        Task<DocumentDb> DeleteDocument(Guid userId, Guid documentId);
    }
}
