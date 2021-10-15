using DocumentService.Boundary.Request;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways.Interfaces
{
    public interface IS3Gateway
    {
        string GetDocumentDownloadPresignedUrl(string key, string fileName);
        string GetDocumentUploadPresignedUrl(string key);
        Task<DocumentUploadResponse> ValidateUploadedDocument(string key);
        Task MoveDocumentToStoreDirectory(string key);
        Task DeleteDocument(string key);
    }
}
