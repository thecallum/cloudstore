using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Infrastructure.Exceptions;
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
        private readonly IDirectoryGateway _directoryGateway;

        public UploadDocumentUseCase(IS3Gateway s3Gateway, IDocumentGateway documentGateway, IDirectoryGateway directoryGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
            _directoryGateway = directoryGateway;
        }

        public async Task<UploadDocumentResponse> Execute(UploadDocumentRequest request, Guid userId)
        {
            if (request.DirectoryId != null)
            {
                var directoryExists = await _directoryGateway.CheckDirectoryExists((Guid) request.DirectoryId, userId);
                if (directoryExists == false) throw new DirectoryNotFoundException();
            }

            var documentId = Guid.NewGuid();

            var response = await _s3Gateway.UploadDocument(request, documentId, userId).ConfigureAwait(false);

            var document = request.ToDomain(userId, documentId, response);

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
