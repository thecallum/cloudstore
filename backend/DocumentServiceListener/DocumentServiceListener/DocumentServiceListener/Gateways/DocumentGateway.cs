using Amazon.DynamoDBv2.DataModel;
using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public class DocumentGateway : IDocumentGateway
    {
        private readonly IDynamoDBContext _context;

        public DocumentGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task DeleteDocuments(List<DocumentDb> documents, Guid userId)
        {
            var taskList = new List<Task>();

            foreach (var document in documents)
            {
                taskList.Add(_context.DeleteAsync<DocumentDb>(userId, document.DocumentId));
            }

            await Task.WhenAll(taskList);
        }

        public async Task<List<DocumentDb>> GetAllDocuments(Guid directoryId, Guid userId)
        {
            var documentList = new List<DocumentDb>();

            var config = new DynamoDBOperationConfig
            {
                IndexName = "DirectoryId_Name",
            };

            var search = _context.QueryAsync<DocumentDb>(directoryId, config);

            do
            {
                var newDocuments = await search.GetNextSetAsync();

                documentList.AddRange(newDocuments);

            } while (search.IsDone == false);

            return documentList;
        }
    }
}
