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
                S3Location = domain.S3Location
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
                S3Location = entity.S3Location
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
    }
}
