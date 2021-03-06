using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
using DocumentService.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.UseCase
{
    public class GetAllDocumentsUseCase : IGetAllDocumentsUseCase
    {
        private readonly IDocumentGateway _documentGateway;

        public GetAllDocumentsUseCase(IDocumentGateway documentGateway)
        {
            _documentGateway = documentGateway;
        }

        public async Task<GetAllDocumentsResponse> Execute(Guid userId, GetAllDocumentsQuery query)
        {
            LogHelper.LogUseCase("GetAllDocumentsUseCase");

            var documentGatewayResponse = await _documentGateway.GetAllDocuments(userId, query.DirectoryId);

            return documentGatewayResponse.ToResponse();
        }
    }
}
