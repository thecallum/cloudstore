using System;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.Gateways
{
    public interface ISnsGateway
    {
        Task PublishDocumentUploadedEvent(User user, Guid documentId);
    }
}
