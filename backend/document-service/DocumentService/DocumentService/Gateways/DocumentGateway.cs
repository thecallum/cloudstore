using Amazon.DynamoDBv2.DataModel;
using DocumentService.Domain;
using DocumentService.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public class DocumentGateway : IDocumentGateway
    {
        private readonly IDynamoDBContext _context;
        public DocumentGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
        }
        public async Task SaveDocument(Document document)
        {
            await _context.SaveAsync(document.ToDatabase());
        }
    }
}
