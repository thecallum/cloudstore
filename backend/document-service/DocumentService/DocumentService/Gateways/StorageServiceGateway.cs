using Amazon.DynamoDBv2.DataModel;
using DocumentService.Domain;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public class StorageServiceGateway : IStorageServiceGateway
    {
        private readonly IDynamoDBContext _context;

        public StorageServiceGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task AddFile(Guid userId, long fileSize)
        {
            var entity = await LoadStorageUsage(userId);

            if (entity == null)
            {
                // create new entity
                entity = new DocumentStorageDb  { UserId = userId };
            }

            entity.StorageUsage += fileSize;

            await SaveStorageUsage(entity);
        }

        public async Task<bool> CanUploadFile(Guid userId, long accountStorageCapacity, long fileSize, long? originalFileSize = null)
        {
            var entity = await LoadStorageUsage(userId);

            // no files have been saved.
            if (entity == null)
            {
                // assert that uploaded file is smaller than capacity
                return fileSize < accountStorageCapacity;
            }

            var expectedUsage = entity.StorageUsage + fileSize;
            if (originalFileSize != null) expectedUsage -= (long)originalFileSize;

            return expectedUsage < accountStorageCapacity;
        }

        public async Task<StorageUsageResponse> GetUsage(Guid userId, long accountStorageCapacity)
        {
            var response = new StorageUsageResponse
            {
                StorageUsage = 0,
                Capacity = accountStorageCapacity
            };

            var entity = await LoadStorageUsage(userId);

            // return default response
            if (entity == null) return response;

            // append usage to response object
            response.StorageUsage = entity.StorageUsage;
            return response;
        }

        public async Task RemoveFile(Guid userId, long fileSize)
        {
            var entity = await LoadStorageUsage(userId);

            // This shouldnt be thrown because record should already exist
            if (entity == null) throw new StorageUsageNotFoundException();

            // replace value
            entity.StorageUsage -= fileSize;

            // update value in databse
            await SaveStorageUsage(entity);
        }

        public async Task ReplaceFile(Guid userId, long fileSize, long originalFileSize)
        {
            var entity = await LoadStorageUsage(userId);

            // This shouldnt be thrown because record should already exist
            if (entity == null) throw new StorageUsageNotFoundException();

            var newStorageUsage = entity.StorageUsage + originalFileSize - fileSize;

            // replace value
            entity.StorageUsage = newStorageUsage;

            // update value in databse
            await SaveStorageUsage(entity);
        }

        private async Task<DocumentStorageDb> LoadStorageUsage(Guid userId)
        {
            return await _context.LoadAsync<DocumentStorageDb>(userId).ConfigureAwait(false);
        }

        private async Task SaveStorageUsage(DocumentStorageDb entity)
        {
            await _context.SaveAsync(entity).ConfigureAwait(false);
        }
    }
}
