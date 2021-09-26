using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public interface IDocumentGateway
    {
        Task SaveDocument(Document document);
    }
}
