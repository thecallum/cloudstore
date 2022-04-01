using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways.Interfaces;
using DocumentService.Services;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class StorageUsageUseCase : IStorageUsageUseCase
    {
        private readonly IDocumentGateway _documentGateway;
        private readonly IStorageUsageCache _cache;

        public StorageUsageUseCase(IDocumentGateway documentGateway, IStorageUsageCache cache)
        {
            _documentGateway = documentGateway;
            _cache = cache;
        }

        public async Task<long> GetUsage(User user)
        {
            var cachedValue = await _cache.GetValue(user.Id);
            if (cachedValue != null) return (long) cachedValue;

            // get value from database
            var storageUsageResponse = await _documentGateway.GetUsage(user);

            await _cache.UpdateValue(user.Id, storageUsageResponse.StorageUsage);

            return storageUsageResponse.StorageUsage;
        }

        public async Task UpdateUsage(User user, long difference)
        {
            var cachedValue = await _cache.GetValue(user.Id);

            long newValue;

            if (cachedValue == null)
            {
                // get value from database
                var storageUsageResponse = await _documentGateway.GetUsage(user);

                newValue = storageUsageResponse.StorageUsage + difference;
            } else
            {
                newValue = (long) cachedValue + difference;
            }
            
            await _cache.UpdateValue(user.Id, newValue);
        }
    }
}
