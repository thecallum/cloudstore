using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class ValidateUploadedDocumentUseCase : IValidateUploadedDocumentUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;

        public ValidateUploadedDocumentUseCase(IS3Gateway s3Gateway, IDocumentGateway documentGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
        }

        public async Task<Document> Execute(Guid userId, Guid documentId, ValidateUploadedDocumentRequest request)
        {
            var key = $"{userId}/{documentId}";

            var documentUploadResponse = await _s3Gateway.ValidateUploadedDocument(key);
            if (documentUploadResponse == null) return null;

            await _s3Gateway.MoveDocumentToStoreDirectory(key);

            var document = new Document
            {
                Id = documentId,
                UserId = userId,
                DirectoryId = request.DirectoryId ?? userId,
                FileSize = documentUploadResponse.FileSize,
                Name = request.FileName,
                S3Location = key,
            };

            await _documentGateway.SaveDocument(document);

            return document;
        }
    }
}
