using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
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

        public async Task<GetAllDirectoriesResponse> Execute(Guid userId, GetAllDirectoriesQuery query)
        {
            var directories = new List<Directory>();

            // if DirectoryId not null, then we are looking for directories within a parent directory.
            // this method will throw exception is parent directory doesnt exist
            var directoryGatewayResponse = await _directoryGateway.GetAllDirectories(userId, query.DirectoryId);

            directories.AddRange(
                directoryGatewayResponse.Select(x => x.ToDomain())
            );

            return new GetAllDirectoriesResponse
            {
                Directories = directories
            };
        }
    }
}
