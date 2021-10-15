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
    public class DeleteDocumentUseCase : IDeleteDocumentUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;
        private readonly IStorageServiceGateway _storageServiceGateway;

        public DeleteDocumentUseCase(IS3Gateway s3Gateway, IDocumentGateway documentGateway, IStorageServiceGateway storageServiceGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
            _storageServiceGateway = storageServiceGateway;
        }

        public async Task Execute(Guid userId, Guid documentId)
        {
            LogHelper.LogUseCase("DeleteDocumentUseCase");

            var document = await _documentGateway.DeleteDocument(userId, documentId);

            await _s3Gateway.DeleteDocument(document.S3Location);
            await _storageServiceGateway.RemoveFile(userId, document.FileSize);
        }
    }
}
