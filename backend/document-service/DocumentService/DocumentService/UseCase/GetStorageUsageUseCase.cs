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
        private readonly IDocumentGateway _documentGateway;

        public GetStorageUsageUseCase(IDocumentGateway documentGateway)
        {
            _documentGateway = documentGateway;
        }

        public async Task<GetStorageUsageResponse> Execute(User user)
        {
            var storageUsageResponse = await _documentGateway.GetUsage(user);

            return new GetStorageUsageResponse
            {
                Capacity = storageUsageResponse.Capacity,
                StorageUsage = storageUsageResponse.StorageUsage
            };
        }
    }
}
