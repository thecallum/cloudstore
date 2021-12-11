using DocumentService.Boundary.Request;
using DocumentService.Factories;
using DocumentService.Gateways.Interfaces;
using DocumentService.Logging;
using System;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public class CreateDirectoryUseCase : ICreateDirectoryUseCase
    {
        private readonly IDirectoryGateway _directoryGateway;

        public CreateDirectoryUseCase(IDirectoryGateway directoryGateway)
        {
            _directoryGateway = directoryGateway;
        }

        public async Task<Guid> Execute(CreateDirectoryRequest request, Guid userId)
        {
            LogHelper.LogUseCase("CreateDirectoryUseCase");

            var directory = request.ToDomain(userId);

            await _directoryGateway.CreateDirectory(directory);

            return directory.Id;
        }
    }
}
