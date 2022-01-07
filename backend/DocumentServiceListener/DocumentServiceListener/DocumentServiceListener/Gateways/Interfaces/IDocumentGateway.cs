using System;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public interface IDocumentGateway
    {
        Task UpdateThumbnail(Guid userId, Guid documentId);
    }
}
