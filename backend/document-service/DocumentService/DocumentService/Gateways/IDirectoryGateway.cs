using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public interface IDirectoryGateway
    {
        Task CreateDirectory(Directory directory);
        Task DeleteDirectory(Guid directoryId, Guid userId);
        Task RenameDirectory(string newName, Guid directoryId, Guid userId);
        Task<IEnumerable<DirectoryDb>> GetAllDirectories(Guid userId, Guid? parentDirectoryId = null);
    }
}
