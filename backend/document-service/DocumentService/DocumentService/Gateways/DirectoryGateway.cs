using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public class DirectoryGateway : IDirectoryGateway
    {
        private readonly IDynamoDBContext _context;

        public DirectoryGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
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

            var directories = await GetAllDirectories(userId, directoryId);

            return directories.Count() > 0;
        }

        public async Task CreateDirectory(Directory directory)
        {
            LogHelper.LogGateway("DirectoryGateway", "CreateDirectory");

            await _context.SaveAsync(directory.ToDatabase());
        }

        public async Task DeleteDirectory(Guid directoryId, Guid userId)
        {
            LogHelper.LogGateway("DirectoryGateway", "DeleteDirectory");

            var existingDirectory = await LoadDirectory(directoryId, userId);
            if (existingDirectory == null) throw new DirectoryNotFoundException();

            await _context.DeleteAsync<DirectoryDb>(userId, directoryId);
        }

        public async Task<IEnumerable<DirectoryDb>> GetAllDirectories(Guid userId, Guid? parentDirectoryId = null)
        {
            LogHelper.LogGateway("DirectoryGateway", "GetAllDirectories");

            // If looking for directories within another directory, 
            // must check that parent directory exists
            if (parentDirectoryId != null)
            {
                var directoryExists = await CheckDirectoryExists((Guid)parentDirectoryId, userId);
                if (directoryExists == false) throw new DirectoryNotFoundException();
            }

            var directoryList = new List<DirectoryDb>();

            // default directory cannot be null, because we cant distinguish between accounts.
            // Instead the default will be the userid
            var selectedParentDirectoryId = (parentDirectoryId != null) ? (Guid)parentDirectoryId : userId;

            var config = new DynamoDBOperationConfig
            {
                IndexName = "DirectoryId_Name",
            };

            var search = _context.QueryAsync<DirectoryDb>(selectedParentDirectoryId, config);

            do
            {
                var newDirectorys = await search.GetNextSetAsync();
                directoryList.AddRange(newDirectorys);

            } while (search.IsDone == false);

            return directoryList;
        }

        public async Task RenameDirectory(string newName, Guid directoryId, Guid userId)
        {
            LogHelper.LogGateway("DirectoryGateway", "RenameDirectory");

            var existingDirectory = await LoadDirectory(directoryId, userId);
            if (existingDirectory == null) throw new DirectoryNotFoundException();

            existingDirectory.Name = newName;

            await _context.SaveAsync(existingDirectory);
        }

        private async Task<DirectoryDb> LoadDirectory(Guid directoryId, Guid userId)
        {
            return await _context.LoadAsync<DirectoryDb>(userId, directoryId);
        }
    }
}
