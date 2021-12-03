using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public interface IStorageServiceGateway
    {
        Task RemoveDocuments(List<DocumentDb> documents, Guid userId);
    }
}
