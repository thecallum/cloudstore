using System.IO;
using System.Threading.Tasks;

namespace AWSServerless1.Gateways
{
    public interface IS3Gateway
    {
        Task<DownloadedImage> DownloadImage(string key);
        Task SaveThumbnail(string key, string contentType, MemoryStream outStream);
    }
}
