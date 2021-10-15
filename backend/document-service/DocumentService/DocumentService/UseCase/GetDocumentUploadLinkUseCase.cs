using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Logging;
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

        public GetDocumentUploadResponse Execute(Guid userId, GetDocumentUploadLinkQuery query)
        {
            LogHelper.LogUseCase("GetDocumentUploadLinkUseCase");

            var documentId = query.ExistingDocumentId ?? Guid.NewGuid();

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
