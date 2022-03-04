using DocumentService.Boundary.Response;
using DocumentService.Domain;
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
        private readonly IDocumentGateway _documentGateway;

        public GetStorageUsageUseCase(IDocumentGateway documentGateway)
        {
            _documentGateway = documentGateway;
        }

        public async Task<StorageUsageResponse> Execute(User user)
        {
            var storageUsageResponse = await _documentGateway.GetUsage(user);

            return new StorageUsageResponse
            {
                Capacity = storageUsageResponse.Capacity,
                StorageUsage = storageUsageResponse.StorageUsage
            };
        }
    }
}
