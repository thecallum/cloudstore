using SixLabors.ImageSharp;
using System.IO;

namespace AWSServerless1.Formatters
{
    public interface IImageFormatter
    {
        public void EditImage(Image image, MemoryStream outStream);
    }
}
