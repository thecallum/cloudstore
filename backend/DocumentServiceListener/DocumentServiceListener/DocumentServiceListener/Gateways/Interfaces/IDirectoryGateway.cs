using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways.Interfaces
{
    public interface IDirectoryGateway
    {
        Task DeleteDirectory(Guid userId, Guid directoryId);
    }
}
