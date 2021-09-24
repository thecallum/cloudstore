using System;
using Amazon.DynamoDBv2.DataModel;

namespace authservice.Infrastructure
{
    [DynamoDBTable("User", LowerCamelCaseProperties = true)]
    public class UserDb
    {
        [DynamoDBHashKey] public string Email { get; set; }

        [DynamoDBProperty] public Guid Id { get; set; }

        [DynamoDBProperty] public string FirstName { get; set; }

        [DynamoDBProperty] public string LastName { get; set; }

        [DynamoDBProperty] public string Hash { get; set; }
    }
}