using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
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
        private readonly ISnsGateway _snsGateway;

        public DeleteDirectoryUseCase(
            IDirectoryGateway directoryGateway, 
            IDocumentGateway documentGateway,
            ISnsGateway snsGateway)
        {
            _directoryGateway = directoryGateway;
            _documentGateway = documentGateway;
            _snsGateway = snsGateway;
        }

        public async Task Execute(DeleteDirectoryQuery query, User user)
        {
            LogHelper.LogUseCase("DeleteDirectoryUseCase");

            // 1. Check directory exists
            var directoryExists = await _directoryGateway.DirectoryExists(query.DirectoryId, user.Id);
            if (!directoryExists) throw new DirectoryNotFoundException();

            // 2. Load all child directories
            var childDirectories = await _directoryGateway.GetAllDirectories(user.Id, query.DirectoryId);

            // 3. Delete any child directories
            if (childDirectories.Any())
            {
                await DeleteAllChildDirectories(user, childDirectories);
            }

            // 4. Publish DeleteDirectory SNS event for primary directory
            await _snsGateway.PublishDeleteDirectoryEvent(user, query.DirectoryId);
        }

        private async Task DeleteAllChildDirectories(User user, IEnumerable<DirectoryDb> childDirectories)
        {
            // order by most parents
            var directoriesOrdered = childDirectories
                .OrderByDescending(x => x.ParentDirectoryIds?.Length ?? 0)
                .Select(x => x.ToDomain());

            foreach (var childDirectory in directoriesOrdered)
            {
                await _snsGateway.PublishDeleteDirectoryEvent(user, childDirectory.Id);
            }
        }
    }
}
