using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authservice.Infrastructure
{
    [DynamoDBTable("User", LowerCamelCaseProperties = true)]
    public class UserDb
    {
        [DynamoDBHashKey]
        public string Email { get; set; }

        [DynamoDBProperty]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public string FirstName { get; set; }

        [DynamoDBProperty]
        public string LastName { get; set; }

        [DynamoDBProperty]
        public string Hash { get; set; }
    }
}
