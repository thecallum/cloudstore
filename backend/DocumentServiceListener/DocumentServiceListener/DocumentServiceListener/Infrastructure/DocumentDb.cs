using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentServiceListener.Infrastructure
{
    [DynamoDBTable("Document", LowerCamelCaseProperties = true)]
    public class DocumentDb
    {
        [DynamoDBHashKey]
        public Guid UserId { get; set; }

        [DynamoDBRangeKey]
        public Guid DocumentId { get; set; }

        [DynamoDBProperty]
        public Guid DirectoryId { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string S3Location { get; set; }

        [DynamoDBProperty]
        public long FileSize { get; set; }
    }
}
