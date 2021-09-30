using Amazon.DynamoDBv2.DataModel;
using System;

namespace DocumentService.Infrastructure
{
    // Default table for CRUD
    [DynamoDBTable("Directory", LowerCamelCaseProperties = true)]
    public class DirectoryDb
    {
        [DynamoDBHashKey] // Partition Key
        public Guid UserId { get; set; }

        [DynamoDBRangeKey] // Range Key
        public Guid DirectoryId { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public Guid ParentDirectoryId { get; set; }
    }
}

