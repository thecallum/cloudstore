using DocumentService.Gateways;
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

        public DeleteDocumentUseCase(IS3Gateway s3Gateway, IDocumentGateway documentGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
        }

        public async Task Execute(Guid userId, Guid documentId)
        {
            LogHelper.LogUseCase("DeleteDocumentUseCase");

            var document = await _documentGateway.DeleteDocument(userId, documentId);

            await _s3Gateway.DeleteDocument(document.S3Location);
        }
    }
}
