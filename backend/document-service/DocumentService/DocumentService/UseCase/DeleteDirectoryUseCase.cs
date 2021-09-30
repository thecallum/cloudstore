using DocumentService.Boundary.Request;
using DocumentService.Gateways;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class DeleteDirectoryUseCase : IDeleteDirectoryUseCase
    {
        private readonly IDirectoryGateway _directoryGateway;

        public DeleteDirectoryUseCase(IDirectoryGateway directoryGateway)
        {
            _directoryGateway = directoryGateway;
        }

        public async Task Execute(DeleteDirectoryQuery query, Guid userId)
        {
            await _directoryGateway.DeleteDirectory(query.Id, userId);
        }
    }
}
