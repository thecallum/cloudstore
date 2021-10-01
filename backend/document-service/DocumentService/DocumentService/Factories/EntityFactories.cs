using DocumentService.Boundary.Request;
using DocumentService.Domain;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Factories
{
    public static class EntityFactories
    {
        public static DocumentDb ToDatabase(this Document domain)
        {
            return new DocumentDb
            {
                DocumentId = domain.Id,
                Name = domain.Name,
                UserId = domain.UserId,
                FileSize = domain.FileSize,
                S3Location = domain.S3Location,
                DirectoryId = domain.DirectoryId
            };
        }

        public static Document ToDomain(this DocumentDb entity)
        {
            return new Document
            {
                Id = entity.DocumentId,
                Name = entity.Name,
                UserId = entity.UserId,
                FileSize = entity.FileSize,
                S3Location = entity.S3Location,
                DirectoryId = entity.DirectoryId
            };
        }

        public static Directory ToDomain(this DirectoryDb entity)
        {
            return new Directory
            {
                DirectoryId = entity.DirectoryId,
                ParentDirectoryId = entity.ParentDirectoryId,
                UserId = entity.UserId,
                Name = entity.Name
            };
        }

        public static DirectoryDb ToDatabase(this Directory domain)
        {
            return new DirectoryDb
            {
                UserId = domain.UserId,
                DirectoryId = domain.DirectoryId,
                Name = domain.Name,
                ParentDirectoryId = domain.ParentDirectoryId
            };
        }

        public static Directory ToDomain(this CreateDirectoryRequest request, Guid userId)
        {
            return new Directory
            {
                UserId = userId,
                DirectoryId = Guid.NewGuid(),
                Name = request.Name,
                ParentDirectoryId = (request.ParentDirectoryId ?? userId)
            };
        }

        public static Document ToDomain(this UploadDocumentRequest request, Guid userId, Guid documentId, DocumentUploadResponse response)
        {
            return new Document
            {
                Id = documentId,
                UserId = userId,
                Name = response.DocumentName,
                FileSize = response.FileSize,
                S3Location = response.S3Location,
                DirectoryId = (request.DirectoryId ?? userId)
            };
        }
    }
}
