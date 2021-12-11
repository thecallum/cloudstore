using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public class DirectoryGateway : IDirectoryGateway
    {
        private readonly DocumentServiceContext _documentServiceContext;

        public DirectoryGateway(DocumentServiceContext documentServiceContext)
        {
            _documentServiceContext = documentServiceContext;
        }

        public async Task<bool> CheckDirectoryExists(Guid directoryId, Guid userId)
        {
            LogHelper.LogGateway("DirectoryGateway", "CheckDirectoryExists");

            var existingDirectory = await LoadDirectory(directoryId, userId);

            return existingDirectory != null;
        }

        public async Task<bool> ContainsChildDirectories(Guid directoryId, Guid userId)
        {
            LogHelper.LogGateway("DirectoryGateway", "ContainsChildDirectories");

            var directories = await _documentServiceContext.Directories
                .Where(x => x.ParentDirectoryId == directoryId && x.UserId == userId)
                .Take(1)
                .ToListAsync();

            return directories.Count() != 0;
        }

        public async Task<IEnumerable<DirectoryDomain>> GetAllDirectories(Guid userId)
        {
            LogHelper.LogGateway("DirectoryGateway", "GetAllDirectories");

            var directories = await _documentServiceContext.Directories
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return directories.Select(x => x.ToDomain());
        }

        public async Task CreateDirectory(DirectoryDomain directory)
        {
            LogHelper.LogGateway("DirectoryGateway", "CreateDirectory");

            _documentServiceContext.Directories.Add(directory.ToDatabase());

            await _documentServiceContext.SaveChangesAsync();
        }

        public async Task DeleteDirectory(Guid directoryId, Guid userId)
        {
            LogHelper.LogGateway("DirectoryGateway", "DeleteDirectory");

            var existingDirectory = await LoadDirectory(directoryId, userId);
            if (existingDirectory == null) throw new DirectoryNotFoundException();

            _documentServiceContext.Directories.Remove(existingDirectory);

            await _documentServiceContext.SaveChangesAsync();
        }

        public async Task RenameDirectory(string newName, Guid directoryId, Guid userId)
        {
            LogHelper.LogGateway("DirectoryGateway", "RenameDirectory");

            var existingDirectory = await LoadDirectory(directoryId, userId);
            if (existingDirectory == null) throw new DirectoryNotFoundException();

            existingDirectory.Name = newName;

            await _documentServiceContext.SaveChangesAsync();
        }

        private async Task<DirectoryDb> LoadDirectory(Guid directoryId, Guid userId)
        {
            var directory = await _documentServiceContext.Directories
                .Where(x => x.Id == directoryId && x.UserId == userId)
                .SingleOrDefaultAsync();

            return directory;
        }
    }
}
