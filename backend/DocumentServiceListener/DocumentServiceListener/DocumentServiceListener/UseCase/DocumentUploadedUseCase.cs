using AWSServerless1.Formatters;
using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Gateways.Interfaces;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AWSServerless1
{
    public class DocumentUploadedUseCase : IDocumentUploadedUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentGateway _documentGateway;
        private readonly IImageFormatter _imageFormatter;
        private readonly IImageLoader _imageLoader;

        // default 100MB
        private const int DefaultMaxImageSize = 104857600;
        private readonly int _maxImageSize;

        public DocumentUploadedUseCase(
            IS3Gateway s3Gateway,
            IDocumentGateway documentGateway,
            IImageFormatter imageFormatter,
            IImageLoader imageLoader)
        {
            _s3Gateway = s3Gateway;
            _documentGateway = documentGateway;
            _imageFormatter = imageFormatter;
            _imageLoader = imageLoader;

            _maxImageSize = SetMaxImageSize();
        }

        private static int SetMaxImageSize()
        {
            // only used for integration tests
            var testingValue = Environment.GetEnvironmentVariable("MaxImageSize") ?? "";
            if (testingValue != "") return int.Parse(testingValue);

            return DefaultMaxImageSize;
        }

        private static Guid? TryParseGuid(Dictionary<string, object> body, string name)
        {
            if (!body.ContainsKey(name)) return null;

            var paramter = body[name].ToString();

            try
            {
                return Guid.Parse(paramter);
            } catch (Exception)
            {
                return null;
            }
        }

        public async Task ProcessMessageAsync(CloudStoreSnsEvent entity)
        {
            var objectId = TryParseGuid(entity.Body, "DocumentId");
            if (objectId == null) throw new ArgumentNullException(nameof(entity));

            Console.WriteLine($"Document Was Uploded: {objectId}");

            var documentKey = $"{entity.User.Id}/{objectId}";

            var objectMetadata = await _s3Gateway.GetObjectMetadata(documentKey);

            if (objectMetadata == null)
            {
                Console.WriteLine($"Document {documentKey} not found in s3");
                return;
            }

            Console.WriteLine($"Object filesize: {objectMetadata.ContentLength}");

            if (objectMetadata.ContentLength > _maxImageSize)
            {
                Console.WriteLine($"Maximum filesize to create thumbnail is 100MB {documentKey}");
                return;
            }

            try
            {
                var downloadedImage = await _s3Gateway.DownloadImage(documentKey);

                Console.WriteLine($"Document {documentKey} was downloaded from s3");

                var image = _imageLoader.LoadImage(downloadedImage.ImageBytes);

                using (var outStream = new MemoryStream())
                {
                    _imageFormatter.EditImage(image, outStream);
                    Console.WriteLine($"Document {documentKey} was formatted");

                    await _s3Gateway.SaveThumbnail(documentKey, downloadedImage.ContentType, outStream);
                    Console.WriteLine($"Document {documentKey} was saved as thumbnail in s3");
                }

                _imageLoader.Dispose();

                await _documentGateway.UpdateThumbnail(entity.User.Id, (Guid) objectId);
            }
            catch (UnknownImageFormatException)
            {
                Console.WriteLine($"Invalid File Format - Cannot Create Thumbnail {documentKey}");
                // Invalid image format. This exception is allowed
                return;
            }
        }
    }

 


}
