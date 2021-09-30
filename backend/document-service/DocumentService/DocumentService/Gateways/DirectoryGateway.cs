using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
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

        public async Task CreateDirectory(Directory directory)
        {
            await _context.SaveAsync<DirectoryDb>(directory.ToDatabase());
        }

        public async Task DeleteDirectory(Guid directoryId, Guid userId)
        {
            var existingDirectory = await LoadDirectory(directoryId, userId);
            if (existingDirectory == null) throw new DirectoryNotFoundException();

            await _context.DeleteAsync<DirectoryDb>(userId, directoryId);
        }

        public async Task<IEnumerable<DirectoryDb>> GetAllDirectories(Guid userId, Guid? parentDirectoryId = null)
        {
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
            var existingDirectory = await LoadDirectory(directoryId, userId);
            if (existingDirectory == null) throw new DirectoryNotFoundException();

            existingDirectory.Name = newName;

            await _context.SaveAsync<DirectoryDb>(existingDirectory);
        }

        private async Task<DirectoryDb> LoadDirectory(Guid directoryId, Guid userId)
        {
            return await _context.LoadAsync<DirectoryDb>(userId, directoryId);
        }
    }
}
