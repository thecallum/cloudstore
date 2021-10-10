using DocumentService.Boundary.Request;
using DocumentService.Gateways;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class RenameDirectoryUseCase : IRenameDirectoryUseCase
    {
        private readonly IDirectoryGateway _directoryGateway;

        public RenameDirectoryUseCase(IDirectoryGateway directoryGateway)
        {
            _directoryGateway = directoryGateway;
        }

        public async Task Execute(RenameDirectoryQuery query, RenameDirectoryRequest request, Guid userId)
        {
            LogHelper.LogUseCase("RenameDirectoryUseCase");

            await _directoryGateway.RenameDirectory(request.Name, query.Id, userId);
        }
    }
}
