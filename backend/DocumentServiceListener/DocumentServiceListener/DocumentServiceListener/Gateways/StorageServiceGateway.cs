using Amazon.DynamoDBv2.DataModel;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public class StorageServiceGateway : IStorageServiceGateway
    {
        private readonly IDynamoDBContext _context;

        public StorageServiceGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task RemoveDocuments(List<DocumentDb> documents, Guid userId)
        {
            long totalUsage = 0;

            foreach(var document in documents)
            {
                totalUsage += document.FileSize;
            }

            // get existing entity
            var existingEntity = await _context.LoadAsync<DocumentStorageDb>(userId);

            if (existingEntity == null) throw new StorageUsageNotFoundException();

            // update entity with new value
            existingEntity.StorageUsage -= totalUsage;

            await _context.SaveAsync(existingEntity);
        }
    }
}
