using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public interface IS3Gateway
    {
        Task DeleteDocuments(List<DocumentDb> documents, Guid userId);
    }
}
