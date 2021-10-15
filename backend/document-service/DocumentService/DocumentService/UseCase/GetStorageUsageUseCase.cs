using DocumentService.Boundary.Response;
using DocumentService.Gateways.Interfaces;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class GetStorageUsageUseCase : IGetStorageUsageUseCase
    {
        private readonly IStorageServiceGateway _storageServiceGateway;

        private readonly long _accountStorageCapacity = 1000;

        public GetStorageUsageUseCase(IStorageServiceGateway storageServiceGateway, long? accountStorageCapacity = null)
        {
            _storageServiceGateway = storageServiceGateway;

            if (accountStorageCapacity != null) _accountStorageCapacity = (long)accountStorageCapacity;
        }

        public async Task<GetStorageUsageResponse> Execute(Guid userId)
        {
            var storageUsageResponse = await _storageServiceGateway.GetUsage(userId, _accountStorageCapacity);

            return new GetStorageUsageResponse
            {
                Capacity = storageUsageResponse.Capacity,
                StorageUsage = storageUsageResponse.StorageUsage
            };
        }
    }
}
