using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace AWSServerless1.Formatters
{

    public class ImageFormatter : IImageFormatter
    {
        public void EditImage(Image image, MemoryStream outStream)
        {
            image.Mutate(x => x.Resize(450, 0));

            var encoderOptions = new JpegEncoder()
            {
                //Quantizer = new WuQuantizer<Color>(),
                // IgnoreMetadata = true
                Quality = 90,
            };

            image.SaveAsJpeg(outStream, encoderOptions);
        }
    }
}
