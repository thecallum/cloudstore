using DocumentService.Gateways;
using DocumentService.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase.Interfaces
{
    public class GetDocumentDownloadLinkUseCase : IGetDocumentDownloadLinkUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;

        public GetDocumentDownloadLinkUseCase(IS3Gateway s3Gateway, IDocumentGateway documentGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
        }

        public async Task<string> Execute(Guid userId, Guid documentId)
        {
            var existingDocument = await _documentGateway.GetDocumentById(userId, documentId);
            if (existingDocument == null) throw new DocumentNotFoundException();

            return _s3Gateway.GetDocumentDownloadPresignedUrl(existingDocument.S3Location, existingDocument.Name);
        }

    }
}
