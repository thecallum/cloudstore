using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure
{
    [DynamoDBTable("Document", LowerCamelCaseProperties = true)]
    public class DocumentDb
    {
        [DynamoDBHashKey]
        public Guid UserId { get; set; }

        [DynamoDBRangeKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string S3Location { get; set; }

        [DynamoDBProperty]
        public long FileSize { get; set; }
    }
}
