using DocumentService.Domain;
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
        private readonly ISnsGateway _snsGateway;

        public DeleteDocumentUseCase(
            IS3Gateway s3Gateway, 
            IDocumentGateway documentGateway,
            ISnsGateway snsGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
            _snsGateway = snsGateway;
        }

        public async Task Execute(User user, Guid documentId)
        {
            LogHelper.LogUseCase("DeleteDocumentUseCase");

            var document = await _documentGateway.DeleteDocument(user.Id, documentId);

            await _s3Gateway.DeleteDocument(document.S3Location);
            await _snsGateway.PublishDocumentDeletedEvent(user, documentId);
        }
    }
}
