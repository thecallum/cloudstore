using DocumentService.Boundary.Response;
using DocumentService.Gateways;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class GetDocumentUploadLinkUseCase : IGetDocumentUploadLinkUseCase
    {
        private readonly IS3Gateway _s3Gateway;

        public GetDocumentUploadLinkUseCase(IS3Gateway s3Gateway)
        {
            _s3Gateway = s3Gateway;
        }

        public GetDocumentUploadResponse Execute(Guid userId)
        {
            var documentId = Guid.NewGuid();
            var key = $"{userId}/{documentId}";

            var uploadUrl = _s3Gateway.GetDocumentUploadPresignedUrl(key);

            return new GetDocumentUploadResponse
            {
                DocumentId = documentId,
                UploadUrl = uploadUrl
            };
        }
    }
}
