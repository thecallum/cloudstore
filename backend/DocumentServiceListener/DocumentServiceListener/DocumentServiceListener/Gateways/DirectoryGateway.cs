using Amazon.DynamoDBv2.DataModel;
using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public class DirectoryGateway : IDirectoryGateway
    {
        private readonly IDynamoDBContext _context;

        public DirectoryGateway(IDynamoDBContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task DeleteDirectory(Guid directoryId, Guid userId)
        {
            await _context.DeleteAsync<DirectoryDb>(userId, directoryId);
        }
    }
}
