using Amazon.S3.Util;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.UseCase
{
    public class DirectoryDeletedUseCase : IDirectoryDeletedUseCase
    {
        public Task ProcessMessageAsync(CloudStoreSnsEvent entity)
        {
            throw new NotImplementedException();
        }
    }
}
