using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Gateways;
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

        public async Task<IEnumerable<Document>> Execute(Guid userId)
        {
            var documents = await _documentGateway.GetAllDocuments(userId);

            return documents.Select(x => x.ToDomain());
        }
    }
}
