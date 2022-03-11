using DocumentServiceListener.Gateways.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AWSServerless1.Gateways
{
    public interface IS3Gateway
    {
        Task<ObjectMetadata> GetObjectMetadata(string key);
        Task<DownloadedImage> DownloadImage(string key);
        Task SaveThumbnail(string key, string contentType, MemoryStream outStream);
        Task DeleteThumbnail(string key);
        Task DeleteDocuments(List<string> keys);
    }
}
