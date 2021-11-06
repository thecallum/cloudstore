using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Models;

namespace DocumentService.UseCase
{
    public class ValidateUploadedDocumentUseCase : IValidateUploadedDocumentUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;
        private readonly IStorageServiceGateway _storageServiceGateway;

        public ValidateUploadedDocumentUseCase(
            IS3Gateway s3Gateway, 
            IDocumentGateway documentGateway, 
            IStorageServiceGateway storageServiceGateway)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
            _storageServiceGateway = storageServiceGateway;
        }

        public async Task<Document> Execute(Guid documentId, ValidateUploadedDocumentRequest request, User user)
        {
            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase");

            var key = $"{user.Id}/{documentId}";

            var documentUploadResponse = await _s3Gateway.ValidateUploadedDocument(key);
            if (documentUploadResponse == null) return null;

            var existingDocument = await _documentGateway.GetDocumentById(user.Id, documentId);
            long? existingDocumentFileSize = existingDocument?.FileSize;

            var canUploadDocument = await _storageServiceGateway.CanUploadFile(user, documentUploadResponse.FileSize, existingDocumentFileSize);
            if (canUploadDocument == false) throw new ExceededUsageCapacityException();

            await _s3Gateway.MoveDocumentToStoreDirectory(key);

            // directoryId is ignored if existing document
            // also ignore filename
            var document = existingDocument?.ToDomain() ?? CreateEntity(documentId, user.Id, key, documentUploadResponse, request);
            document.FileSize = documentUploadResponse.FileSize;

            await _documentGateway.SaveDocument(document);

            if (existingDocument == null)
            {
                await _storageServiceGateway.AddFile(user.Id, documentUploadResponse.FileSize);
            } else
            {
                await _storageServiceGateway.ReplaceFile(user.Id, documentUploadResponse.FileSize, existingDocument.FileSize);
            }

            return document;
        }

        private Document CreateEntity(
            Guid documentId, 
            Guid userId, 
            string key,
            DocumentUploadResponse documentUploadResponse, 
            ValidateUploadedDocumentRequest request)
        {
            return new Document
            {
                Id = documentId,
                UserId = userId,
                DirectoryId = request.DirectoryId ?? userId,
                FileSize = documentUploadResponse.FileSize,
                Name = request.FileName,
                S3Location = key,
            };
        }
    }
}
