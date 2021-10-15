using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Tests.Infrastructure
{
    public static class DatabaseBuilder
    {
        public static async Task CreateDocumentTable(IAmazonDynamoDB dynamoDb)
        {
            var request = new CreateTableRequest
            {
                TableName = "Document",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "userId",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "documentId",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "name",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "directoryId",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "userId",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "documentId",
                        KeyType = "RANGE" //Sort key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                }
            };

            request.GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
            {
                new GlobalSecondaryIndex
                {
                    IndexName = "DirectoryId_Name",
                    Projection = new Projection {
                        ProjectionType = ProjectionType.ALL
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "directoryId",
                            KeyType = "HASH"
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "name",
                            KeyType = "RANGE"
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 2,
                        WriteCapacityUnits = 2
                    }
                }
            };

            await dynamoDb.CreateTableAsync(request).ConfigureAwait(false);
        }

        public static async Task CreateDirectoryTable(IAmazonDynamoDB dynamoDb)
        {
            var request = new CreateTableRequest
            {
                TableName = "Directory",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "userId",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "directoryId",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "name",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "parentDirectoryId",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "userId",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "directoryId",
                        KeyType = "RANGE" //Sort key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                }
            };

            request.GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
            {
                new GlobalSecondaryIndex
                {
                    IndexName = "DirectoryId_Name",
                    Projection = new Projection {
                        ProjectionType = ProjectionType.ALL
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "parentDirectoryId",
                            KeyType = "HASH"
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "name",
                            KeyType = "RANGE"
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 2,
                        WriteCapacityUnits = 2
                    }
                }
            };

            await dynamoDb.CreateTableAsync(request).ConfigureAwait(false);
        }

        public static async Task CreateDocumentStorageTable(IAmazonDynamoDB dynamoDb)
        {
            var request = new CreateTableRequest
            {
                TableName = "DocumentStorage",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "userId",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "userId",
                        KeyType = "HASH" //Partition key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 2
                }
            };

            await dynamoDb.CreateTableAsync(request).ConfigureAwait(false);
        }
    }
 }
