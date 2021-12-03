using Amazon.Lambda.Core;
using DocumentServiceListener.Boundary.Request;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.UseCase
{
    public class DeleteDirectoryUseCase : IDeleteDirectoryUseCase
    {
        private readonly IDirectoryGateway _directoryGateway;
        private readonly IDocumentGateway _documentGateway;
        private readonly IStorageServiceGateway _storageServiceGateway;
        private readonly IS3Gateway _s3Gateway;

        public DeleteDirectoryUseCase(
            IDirectoryGateway directoryGateway,
            IDocumentGateway documentGateway,
            IStorageServiceGateway storageServiceGateway,
            IS3Gateway s3Gateway)
        {
            _directoryGateway = directoryGateway;
            _documentGateway = documentGateway;
            _storageServiceGateway = storageServiceGateway;
            _s3Gateway = s3Gateway;
        }

        public async Task ProcessMessageAsync(SnsEvent message)
        {
            LambdaLogger.Log("Calling DeleteDirectoryUseCase");

            // TargetId is the Id of the directory.
            // 1. Get all documents in directory
            var documents = await _documentGateway.GetAllDocuments(message.TargetId, message.User.Id);

            if (documents.Count > 0)
            {
                await DeleteDocuments(documents, message.User.Id);
            }

            // 5. Delete the directory from DynamoDb
            await _directoryGateway.DeleteDirectory(message.TargetId, message.User.Id);
        }

        private async Task DeleteDocuments(List<DocumentDb> documents, Guid userId)
        {
            var taskList = new List<Task>();

            // 2. Delete the documents from S3
            taskList.Add(_s3Gateway.DeleteDocuments(documents, userId));

            // 3. Delete the documents from DynamoDb
            taskList.Add(_documentGateway.DeleteDocuments(documents, userId));

            // 4. Update storage usage total
            taskList.Add(_storageServiceGateway.RemoveDocuments(documents, userId));

            await Task.WhenAll(taskList);
        }
    }
}
