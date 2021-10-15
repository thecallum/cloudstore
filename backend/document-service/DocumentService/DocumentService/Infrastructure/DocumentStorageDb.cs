using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Infrastructure
{
    [DynamoDBTable("DocumentStorage", LowerCamelCaseProperties = true)]
    public class DocumentStorageDb
    { 
        [DynamoDBHashKey]
        public Guid UserId { get; set; }

        [DynamoDBProperty]
        public long StorageUsage { get; set; }
    }
}
