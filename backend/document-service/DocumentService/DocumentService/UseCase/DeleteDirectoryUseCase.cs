using DocumentService.Boundary.Request;
using DocumentService.Gateways;
using DocumentService.Infrastructure.Exceptions;
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
        private readonly IDocumentGateway _documentGateway;

        public DeleteDirectoryUseCase(IDirectoryGateway directoryGateway, IDocumentGateway documentGateway)
        {
            _directoryGateway = directoryGateway;
            _documentGateway = documentGateway;
        }

        public async Task Execute(DeleteDirectoryQuery query, Guid userId)
        {
            var directoryContainsFiles = await _documentGateway.DirectoryContainsFiles(userId, query.DirectoryId);

            // Need to implement recursive delete functionality, but it is too complicated for now
            if (directoryContainsFiles) throw new DirectoryContainsDocumentsException();

            var directoryContainsChildDirectories = await _directoryGateway.ContainsChildDirectories(query.DirectoryId, userId);
            if (directoryContainsChildDirectories) throw new DirectoryContainsChildDirectoriesException();

            await _directoryGateway.DeleteDirectory(query.DirectoryId, userId);
        }
    }
}
