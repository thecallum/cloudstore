using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using System;
using System.Threading.Tasks;
using TokenService.Models;
using User = TokenService.Models.User;

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

        public async Task<DocumentResponse> Execute(Guid documentId, ValidateUploadedDocumentRequest request, User user)
        {
            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase");

            var key = GetS3Key(user.Id, documentId);

            var documentUploadResponse = await _s3Gateway.ValidateUploadedDocument(key);
            if (documentUploadResponse == null) return null;

            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase - Checking document uploaded");

            var existingDocument = await _documentGateway.GetDocumentById(user.Id, documentId);

            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase - loading existing document");

            // check if document can be uploaded / updated
            var canUploadDocument = await _documentGateway.CanUploadFile(user, documentUploadResponse.FileSize, existingDocument?.FileSize);
            if (canUploadDocument == false) throw new ExceededUsageCapacityException();

            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase - Within storage capacity");

            if (existingDocument == null)
            {
                LogHelper.LogUseCase("ValidateUploadedDocumentUseCase - Uploading new document");

                var newDocument = new DocumentDomain
                {
                    Id = documentId,
                    UserId = user.Id,
                    DirectoryId = request.DirectoryId,
                    FileSize = documentUploadResponse.FileSize,
                    Name = request.FileName,
                    S3Location = key,
                };

                await UploadNewDocument(newDocument, key);

                return newDocument.ToResponse();
            }

            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase - Updating existing document");

            await UpdateExistingDocument(existingDocument, documentUploadResponse, key);
            return existingDocument.ToResponse();
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
