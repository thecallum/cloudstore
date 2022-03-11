using DocumentServiceListener.Gateways.Interfaces;
using DocumentServiceListener.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
{
    public class DirectoryGateway : IDirectoryGateway
    {
        private readonly DocumentServiceContext _documentStorageContext;

        public DirectoryGateway(DocumentServiceContext documentStorageContext)
        {
            _documentStorageContext = documentStorageContext;
        }

        public async Task DeleteDirectory(Guid userId, Guid directoryId)
        {
            var directory = await GetDirectory(userId, directoryId);
            if (directory == null) return;

            _documentStorageContext.Directories.Remove(directory);

            await _documentStorageContext.SaveChangesAsync();
        }

        private async Task<DirectoryDb> GetDirectory(Guid userId, Guid directoryId)
        {
            return await _documentStorageContext.Directories
                .Where(x => x.UserId == userId && x.Id == directoryId)
                .SingleOrDefaultAsync();
        }
    }
}
