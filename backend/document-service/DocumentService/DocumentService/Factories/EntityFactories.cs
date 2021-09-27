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
                Id = domain.Id,
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
                Id = entity.Id,
                Name = entity.Name,
                UserId = entity.UserId,
                FileSize = entity.FileSize,
                S3Location = entity.S3Location
            };
        }
    }
}
