using Amazon.DynamoDBv2.DataModel;
using DocumentService.Factories;
using DocumentService.Gateways.Interfaces;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.Logging;
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

        public async Task<DocumentDb> DeleteDocument(Guid userId, Guid documentId)
        {
            LogHelper.LogGateway("DocumentGateway", "DeleteDocument");

            var existingDocument = await GetDocumentById(userId, documentId);
            if (existingDocument == null) throw new DocumentNotFoundException();

            await _context.DeleteAsync<DocumentDb>(userId, documentId);

            return existingDocument;
        }

        public async Task<bool> DirectoryContainsFiles(Guid userId, Guid directoryId)
        {
            LogHelper.LogGateway("DocumentGateway", "DirectoryContainsFiles");

            var documents = await GetAllDocuments(userId, directoryId);

            return documents.Count() > 0;
        }

        public async Task<IEnumerable<DocumentDb>> GetAllDocuments(Guid userId, Guid? directoryId = null)
        {
            LogHelper.LogGateway("DocumentGateway", "GetAllDocuments");

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

        public async Task<DocumentDb> GetDocumentById(Guid userId, Guid documentId)
        {
            LogHelper.LogGateway("DocumentGateway", "GetDocumentById");

            return await _context.LoadAsync<DocumentDb>(userId, documentId);
        }

        public async Task SaveDocument(Document document)
        {
            LogHelper.LogGateway("DocumentGateway", "SaveDocument");

            await _context.SaveAsync(document.ToDatabase());
        }
    }
}
