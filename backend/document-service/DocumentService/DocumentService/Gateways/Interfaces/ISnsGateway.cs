using DocumentService.Domain;
using System;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public interface ISnsGateway
    {
        Task PublishDocumentUploadedEvent(User user, Guid documentId);
        Task PublishDocumentDeletedEvent(User user, Guid documentId);
    }
}
