using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Infrastructure;
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

        public async Task<IEnumerable<DocumentDb>> GetAllDocuments(Guid userId)
        {
            var documentList = new List<DocumentDb>();

            var search = _context.QueryAsync<DocumentDb>(userId);

            do
            {
                var newDocuments = await search.GetNextSetAsync();
                
                
                documentList.AddRange(newDocuments);

            } while (search.IsDone == false);

            return documentList;
        }

        public async Task SaveDocument(Document document)
        {
            await _context.SaveAsync(document.ToDatabase());
        }
    }
}
