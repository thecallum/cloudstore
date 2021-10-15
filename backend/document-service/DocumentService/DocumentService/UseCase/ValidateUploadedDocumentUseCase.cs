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

namespace DocumentService.UseCase
{
    public class ValidateUploadedDocumentUseCase : IValidateUploadedDocumentUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;
        private readonly IStorageServiceGateway _storageServiceGateway;

        private readonly long _accountStorageCapacity = 1000;

        public ValidateUploadedDocumentUseCase(
            IS3Gateway s3Gateway, 
            IDocumentGateway documentGateway, 
            IStorageServiceGateway storageServiceGateway,
            long? accountStorageCapacity = null)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
            _storageServiceGateway = storageServiceGateway;

            if (accountStorageCapacity != null) _accountStorageCapacity = (long) accountStorageCapacity;
        }

        public async Task<Document> Execute(Guid userId, Guid documentId, ValidateUploadedDocumentRequest request)
        {
            LogHelper.LogUseCase("ValidateUploadedDocumentUseCase");

            var key = $"{userId}/{documentId}";

            var documentUploadResponse = await _s3Gateway.ValidateUploadedDocument(key);
            if (documentUploadResponse == null) return null;

            var existingDocument = await _documentGateway.GetDocumentById(userId, documentId);
            long? existingDocumentFileSize = existingDocument?.FileSize;

            var canUploadDocument = await _storageServiceGateway.CanUploadFile(userId, _accountStorageCapacity, documentUploadResponse.FileSize, existingDocumentFileSize);
            if (canUploadDocument == false) throw new ExceededUsageCapacityException();

            await _s3Gateway.MoveDocumentToStoreDirectory(key);

            // directoryId is ignored if existing document
            // also ignore filename
            var document = existingDocument?.ToDomain() ?? CreateEntity(documentId, userId, key, documentUploadResponse, request);
            document.FileSize = documentUploadResponse.FileSize;

            await _documentGateway.SaveDocument(document);

            if (existingDocument == null)
            {
                await _storageServiceGateway.AddFile(userId, documentUploadResponse.FileSize);
            } else
            {
                await _storageServiceGateway.ReplaceFile(userId, documentUploadResponse.FileSize, existingDocument.FileSize);
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
