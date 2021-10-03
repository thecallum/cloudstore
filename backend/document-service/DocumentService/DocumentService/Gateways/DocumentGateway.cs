using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Document = DocumentService.Domain.Document;

namespace DocumentService.Gateways
{
    public class DocumentGateway : IDocumentGateway
    {
        private readonly IDynamoDBContext _context;
        public DocumentGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task<bool> DirectoryContainsFiles(Guid userId, Guid directoryId)
        {
            var documents = await GetAllDocuments(userId, directoryId);

            return documents.Count() > 0;
        }

        public async Task<IEnumerable<DocumentDb>> GetAllDocuments(Guid userId, Guid? directoryId = null)
        {
            var documentList = new List<DocumentDb>();

            var selectedDirectoryId = (directoryId != null) ? (Guid)directoryId : userId;

            var config = new DynamoDBOperationConfig
            {
                IndexName = "DirectoryId_Name",
            };

            var search = _context.QueryAsync<DocumentDb>(selectedDirectoryId, config);

            //var queryConfig = new QueryOperationConfig
            //{
            //    IndexName = "DirectoryId_Name",
            //    Limit = 2,
            //    Select = SelectValues.AllAttributes,
            //    //Filter = new QueryFilter("directoryId", QueryOperator.Equal, selectedDirectoryId),

            //    PaginationToken = null,
            //    KeyExpression = new Expression
            //    {
            //        ExpressionStatement = "directoryId = :directoryId"
            //    },
            //};

            //// queryConfig.KeyExpression.ExpressionAttributeNames.Add("directoryId", $"\"S\": \"{selectedDirectoryId}\"");
            //queryConfig.KeyExpression.ExpressionAttributeValues.Add(":directoryId", $"{selectedDirectoryId}");

            //// Add(":directoryId", selectedDirectoryId.ToString());

            //var search = _context.FromQueryAsync<DocumentDb>(queryConfig);

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
