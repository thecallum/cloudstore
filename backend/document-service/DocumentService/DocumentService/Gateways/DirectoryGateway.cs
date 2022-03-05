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

        public async Task<bool> DirectoryExists(Guid directoryId, Guid userId)
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

        public async Task<IEnumerable<DirectoryDb>> GetAllDirectories(Guid userId, Guid? parentDirectoryId = null)
        {
            LogHelper.LogGateway("DirectoryGateway", "GetAllDirectories");

            if (parentDirectoryId == null)
            {
                // return directories with any parent
                return await _documentServiceContext.Directories
                    .Where(x => x.UserId == userId)
                    .ToListAsync();
            }

            // return directories with a specific parentDirectory
            return await _documentServiceContext.Directories
                .Where(x => 
                    x.ParentDirectoryIds.Contains(parentDirectoryId.ToString()) &&
                    x.UserId == userId)
                    .ToListAsync();
        }

        public async Task CreateDirectory(DirectoryDomain directory)
        {
            LogHelper.LogGateway("DirectoryGateway", "CreateDirectory");

            var parentDirectory = await GetParentDirectory(directory.ParentDirectoryId);

            _documentServiceContext.Directories.Add(directory.ToDatabase(parentDirectory));

            await _documentServiceContext.SaveChangesAsync();
        }

        private async Task<DirectoryDb> GetParentDirectory(Guid? parentDirectoryID)
        {
            if (parentDirectoryID == null) return null;

            return await _documentServiceContext.Directories.FindAsync(parentDirectoryID);
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
