using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using System;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.UseCase
{
    public class ValidateUploadedDocumentUseCase : IValidateUploadedDocumentUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;

        public ValidateUploadedDocumentUseCase(
            IS3Gateway s3Gateway,
            IDocumentGateway documentGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
        }

        public async Task<DocumentDomain> Execute(Guid documentId, ValidateUploadedDocumentRequest request, User user)
        {
            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase");

            var key = GetS3Key(user.Id, documentId);

            var documentUploadResponse = await _s3Gateway.ValidateUploadedDocument(key);
            if (documentUploadResponse == null) return null;

            var existingDocument = await _documentGateway.GetDocumentById(user.Id, documentId);

            // check if document can be uploaded / updated
            var canUploadDocument = await _documentGateway.CanUploadFile(user, documentUploadResponse.FileSize, existingDocument?.FileSize);
            if (canUploadDocument == false) throw new ExceededUsageCapacityException();


            if (existingDocument == null)
            {
                var newDocument = new DocumentDomain
                {
                    Id = documentId,
                    UserId = user.Id,
                    DirectoryId = request.DirectoryId,
                    FileSize = documentUploadResponse.FileSize,
                    Name = request.FileName,
                    S3Location = key,
                };

                return await UploadNewDocument(newDocument, key);
            }

            return await UpdateExistingDocument(existingDocument, documentUploadResponse, key);
        }

        private async Task<DocumentDomain> UpdateExistingDocument(DocumentDomain existingDocument, DocumentUploadResponse documentUploadResponse, string key)
        {
            await _s3Gateway.MoveDocumentToStoreDirectory(key);

            // update document
            existingDocument.FileSize = documentUploadResponse.FileSize;

            await _documentGateway.UpdateDocument(existingDocument);

            return existingDocument;
        }

        private string GetS3Key(Guid userId, Guid documentId)
        {
            return $"{userId}/{documentId}";
        }

        private async Task<DocumentDomain> UploadNewDocument(DocumentDomain newDocument, string key)
        {
            await _s3Gateway.MoveDocumentToStoreDirectory(key);

            await _documentGateway.SaveDocument(newDocument);

            return newDocument;
        }
    }
}
