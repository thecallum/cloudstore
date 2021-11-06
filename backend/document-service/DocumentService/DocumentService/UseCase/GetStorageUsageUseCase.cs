using DocumentService.Boundary.Response;
using DocumentService.Gateways.Interfaces;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.UseCase
{
    public class GetStorageUsageUseCase : IGetStorageUsageUseCase
    {
        private readonly IStorageServiceGateway _storageServiceGateway;

        public GetStorageUsageUseCase(IStorageServiceGateway storageServiceGateway)
        {
            _storageServiceGateway = storageServiceGateway;
        }

        public async Task<GetStorageUsageResponse> Execute(User user)
        {
            var storageUsageResponse = await _storageServiceGateway.GetUsage(user);

            return new GetStorageUsageResponse
            {
                Capacity = storageUsageResponse.Capacity,
                StorageUsage = storageUsageResponse.StorageUsage
            };
        }
    }
}
