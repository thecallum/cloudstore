using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class GetAllDirectoriesUseCase : IGetAllDirectoriesUseCase
    {
        private readonly IDirectoryGateway _directoryGateway;

        public GetAllDirectoriesUseCase(IDirectoryGateway directoryGateway)
        {
            _directoryGateway = directoryGateway;
        }

        public async Task<GetAllDirectoriesResponse> Execute(Guid userId)
        {
            LogHelper.LogUseCase("GetAllDirectoriesUseCase");

            var directoryGatewayResponse = await _directoryGateway.GetAllDirectories(userId);

            return directoryGatewayResponse.ToResponse();
        }
    }
}
