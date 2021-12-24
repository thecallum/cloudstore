using AWSServerless1.Formatters;
using AWSServerless1.Gateways;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;
using static Amazon.S3.Util.S3EventNotification;

namespace AWSServerless1
{
    public class DocumentUploadedUseCase : IDocumentUploadedUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IImageFormatter _imageFormatter;

        public DocumentUploadedUseCase(
            IS3Gateway s3Gateway,
            IImageFormatter imageFormatter)
        {
            _s3Gateway = s3Gateway;
            _imageFormatter = imageFormatter;
        }

        private string GetObjectKey(string entityKey)
        {
            return entityKey.Replace("store/", "");
        }

        public async Task ProcessMessageAsync(S3Entity entity)
        {
            Console.WriteLine($"Document Was Uploded: {entity.Object.Key}");

            // remove 'store/' from key name
            var key = GetObjectKey(entity.Object.Key);

            try
            {

                var downloadedImage = await _s3Gateway.DownloadImage(key);
                if (downloadedImage == null)
                {
                    Console.Write("Document not found in s3");
                    return;
                }

                Console.WriteLine("Document was downloaded from s3");

                using var image = Image.Load(downloadedImage.ImageBytes);
                using (var outStream = new MemoryStream())
                {
                    _imageFormatter.EditImage(image, outStream);
                    Console.WriteLine("Document was eddited");
                    await _s3Gateway.SaveThumbnail(key, downloadedImage.ContentType, outStream);
                    Console.WriteLine("Document was saved as thumbnail in s3");
                }

            }
            catch (UnknownImageFormatException)
            {
                Console.WriteLine("Invalid File Format - Cannot Create Thumbnail");
                // Invalid image format. This exception is allowed
                return;
            }
            catch (DocumentTooLargeException)
            {
                Console.WriteLine("Maximum filesize to create thumbnail is 100MB");
                // Invalid image format. This exception is allowed
                return;
            }
        }
    }
}
