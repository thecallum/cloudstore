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
                DirectoryId = domain.DirectoryId,
                Thumbnail = domain.Thumbnail
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
                DirectoryId = entity.DirectoryId,
                Thumbnail = entity.Thumbnail
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

#nullable enable
        private static string? GenerateParentDirectoryIds(DirectoryDb? parentDirectory)
        {
            // if null, must be root directory. cannot be parent of parent
            if (parentDirectory == null) return null;

            // parent must exist, return "{...so on}{parentOfParentId}/{parentId}"

            if (parentDirectory.ParentDirectoryId == null)
            {
                // parent is a root directory. therefore, only return its id
                return parentDirectory.Id.ToString();
            }

            // parent directory must have parent directories
            return $"{parentDirectory.ParentDirectoryIds}/{parentDirectory.Id}";
        }

        public static DirectoryDb ToDatabase(this DirectoryDomain domain, DirectoryDb? parentDirectory = null)
        {
            return new DirectoryDb
            {
                Id = domain.Id,
                UserId = domain.UserId,
                Name = domain.Name,
                ParentDirectoryId = domain.ParentDirectoryId,
                ParentDirectoryIds = GenerateParentDirectoryIds(parentDirectory)
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
