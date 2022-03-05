using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentService.Gateways.Interfaces
{
    public interface IDirectoryGateway
    {
        Task CreateDirectory(DirectoryDomain directory);
        Task RenameDirectory(string newName, Guid directoryId, Guid userId);
        Task<IEnumerable<DirectoryDb>> GetAllDirectories(Guid userId, Guid? parentDirectoryId = null);
        Task<bool> DirectoryExists(Guid directoryId, Guid userId);
        Task<bool> ContainsChildDirectories(Guid directoryId, Guid userId);
    }
}
