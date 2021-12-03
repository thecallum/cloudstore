using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public interface IDocumentGateway
    {
        Task<List<DocumentDb>> GetAllDocuments(Guid directoryId, Guid userId);
        Task DeleteDocuments(List<DocumentDb> documents, Guid userId);
    }
}
