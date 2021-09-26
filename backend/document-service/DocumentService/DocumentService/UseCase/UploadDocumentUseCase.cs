using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class UploadDocumentUseCase : IUploadDocumentUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;

        public UploadDocumentUseCase(IS3Gateway s3Gateway, IDocumentGateway documentGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
        }
        public async Task<UploadDocumentResponse> Execute(UploadDocumentRequest request, Guid userId)
        {
            var documentId = Guid.NewGuid();

            var response = await _s3Gateway.UploadDocument(request, documentId, userId).ConfigureAwait(false);

            var document = new Document
            {
                Id = documentId,
                UserId = userId,
                Name = response.DocumentName,
                S3Location = response.S3Location,
                FileSize = response.FileSize
            };

            await _documentGateway.SaveDocument(document);

            return new UploadDocumentResponse
            {
                DocumentId = documentId,
                Name = response.DocumentName,
                S3Location = response.S3Location
            };
        }
    }
}
