using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public interface IDirectoryGateway
    {
        Task DeleteDirectory(Guid directoryId, Guid userId);
    }
}
