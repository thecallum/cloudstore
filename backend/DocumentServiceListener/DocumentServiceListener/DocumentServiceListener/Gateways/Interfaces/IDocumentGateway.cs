using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public interface IDocumentGateway
    {
        Task UpdateThumbnail(Guid userId, Guid documentId);
        Task<IEnumerable<DocumentDb>> GetAllDocuments(Guid userId, Guid directoryId);
        Task DeleteAllDocuments(Guid userId, Guid directoryId);
    }
}
