using DocumentService.Boundary.Response;
using DocumentService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Factories
{
    public static class ResponseFactory
    {
        public static GetAllDirectoriesResponse ToResponse(this IEnumerable<DirectoryDomain> domainDirectories)
        {
            return new GetAllDirectoriesResponse
            {
                Directories = domainDirectories.Select(x => x.ToResponse()).ToList()
            };
        }

        public static DirectoryResponse ToResponse(this DirectoryDomain domain)
        {
            return new DirectoryResponse
            {
                Id = domain.Id,
                Name = domain.Name,
                ParentDirectoryId = domain.ParentDirectoryId
            };
        }

        public static GetAllDocumentsResponse ToResponse(this IEnumerable<DocumentDomain> domainDocuments)
        {
            return new GetAllDocumentsResponse
            {
                Documents = domainDocuments.Select(x => x.ToResponse()).ToList()
            };
        }

        public static DocumentResponse ToResponse(this DocumentDomain domain)
        {
            return new DocumentResponse
            {
                Id = domain.Id,
                DirectoryId = domain.DirectoryId,
                FileSize = domain.FileSize,
                Name = domain.Name,
                S3Location = domain.S3Location
            };
        }
    }
}
