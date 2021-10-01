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
    public class GetAllDocumentsUseCase : IGetAllDocumentsUseCase
    {
        private readonly IDocumentGateway _documentGateway;
        private readonly IDirectoryGateway _directoryGateway;

        public GetAllDocumentsUseCase(IDocumentGateway documentGateway, IDirectoryGateway directoryGateway)
        {
            _documentGateway = documentGateway;
            _directoryGateway = directoryGateway;
        }

        public async Task<GetAllDocumentsResponse> Execute(Guid userId, GetAllDocumentsQuery query)
        {
            var documents = new List<Document>();
            var directories = new List<Directory>();

            // if DirectoryId not null, then we are looking for directories within a directory.
            // this method will throw exception is parent directory doesnt exist
            var directoryGatewayResponse = await _directoryGateway.GetAllDirectories(userId, query.DirectoryId);
            directories.AddRange(
                directoryGatewayResponse.Select(x => x.ToDomain())
            );

            // if parent directory doesnt exist, this wont run because an exception will be thrown before

            var documentGatewayResponse = await _documentGateway.GetAllDocuments(userId, query.DirectoryId);
            documents.AddRange(
                documentGatewayResponse.Select(x => x.ToDomain())
            );

            return new GetAllDocumentsResponse
            {
                Documents = documents,
                Directories = directories
            };
        }
    }
}
