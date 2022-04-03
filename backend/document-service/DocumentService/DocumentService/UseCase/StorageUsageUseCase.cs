using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways.Interfaces;
using DocumentService.Services;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentService.Logging;

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
            LogHelper.LogUseCase("GetUsage");

            var cachedValue = await _cache.GetValue(user.Id);
            Console.WriteLine($"Reading StorageUsage cachedValue: [{cachedValue}] [{user.Id}]");

            if (cachedValue != null) return (long) cachedValue;

            // get value from database
            var storageUsageResponse = await _documentGateway.GetUsage(user);
            Console.WriteLine($"Reading StorageUsage from databse");

            await _cache.UpdateValue(user.Id, storageUsageResponse.StorageUsage);
            Console.WriteLine($"Saving database response to cache: [{storageUsageResponse.StorageUsage}]");

            return storageUsageResponse.StorageUsage;
        }

        public async Task UpdateUsage(User user, long difference)
        {
            LogHelper.LogUseCase("UpdateUsage");

            var cachedValue = await _cache.GetValue(user.Id);

            Console.WriteLine($"Reading StorageUsage cachedValue: [{cachedValue}] [{user.Id}]");

            long newValue;

            if (cachedValue == null)
            {
                Console.WriteLine($"Reading storageUsage from database");

                // get value from database
                var storageUsageResponse = await _documentGateway.GetUsage(user);

                newValue = storageUsageResponse.StorageUsage + difference;
            } else
            {
                newValue = (long) cachedValue + difference;

                Console.WriteLine($"Updating using cachedValue: [{newValue}]");
            }
            
            Console.WriteLine($"Updating storageUsage: [{newValue}]");

            await _cache.UpdateValue(user.Id, newValue);
        }
    }
}
