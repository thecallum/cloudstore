using DocumentService.Boundary.Request;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public interface IS3Gateway
    {
        Task<DocumentUploadResponse> UploadDocument(UploadDocumentRequest request, Guid documentId, Guid userId);
        string GetDocumentPresignedUrl(string key);
    }
}
