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
        Task<IEnumerable<DocumentDb>> GetAllDocuments(Guid userId);
    }
}
