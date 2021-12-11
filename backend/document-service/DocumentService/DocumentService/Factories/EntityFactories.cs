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
        public static DocumentDb ToDatabase(this DocumentDomain domain)
        {
            return new DocumentDb
            {
                Id = domain.Id,
                Name = domain.Name,
                UserId = domain.UserId,
                FileSize = domain.FileSize,
                S3Location = domain.S3Location,
                DirectoryId = domain.DirectoryId
            };
        }

        public static DocumentDomain ToDomain(this DocumentDb entity)
        {
            return new DocumentDomain
            {
                Id = entity.Id,
                Name = entity.Name,
                UserId = entity.UserId,
                FileSize = entity.FileSize,
                S3Location = entity.S3Location,
                DirectoryId = entity.DirectoryId
            };
        }

        public static DirectoryDomain ToDomain(this DirectoryDb entity)
        {
            return new DirectoryDomain
            {
                Id = entity.Id,
                ParentDirectoryId = entity.ParentDirectoryId,
                UserId = entity.UserId,
                Name = entity.Name
            };
        }

        public static DirectoryDb ToDatabase(this DirectoryDomain domain)
        {
            return new DirectoryDb
            {
                Id = domain.Id,
                UserId = domain.UserId,
                Name = domain.Name,
                ParentDirectoryId = domain.ParentDirectoryId
            };
        }

        public static DirectoryDomain ToDomain(this CreateDirectoryRequest request, Guid userId)
        {
            return new DirectoryDomain
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = request.Name,
                ParentDirectoryId = request.ParentDirectoryId
            };
        }

    }
}
