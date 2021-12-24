using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless1.Gateways
{
    public class S3Gateway : IS3Gateway
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;
        const int MaxImageSize = 104857600;

        public S3Gateway(IAmazonS3 amazonS3)
        {
            _bucketName = "uploadfromcs";
            _amazonS3 = amazonS3;
        }

        private async Task<GetObjectMetadataResponse> GetObjectMetaData(string key)
        {
            Console.WriteLine($"Getting metadata for object {key}");
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = $"store/{key}"
            };

            try
            {
                var response = await _amazonS3.GetObjectMetadataAsync(request);

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DownloadedImage> DownloadImage(string key)
        {
            var objectMetaData = await GetObjectMetaData(key);
            if (objectMetaData == null) return null;

            Console.WriteLine($"Object filesize: {objectMetaData.ContentLength}");
            if (objectMetaData.ContentLength > MaxImageSize)
            {
                throw new DocumentTooLargeException();
            }

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"store/{key}"
            };

            byte[] imageBytes = null;
            var contentType = "";

            using (var objectResponse = await _amazonS3.GetObjectAsync(request))
            {
                using (var ms = new MemoryStream())
                {
                    contentType = objectResponse.Headers.ContentType;
                    await objectResponse.ResponseStream.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }
            }

            return new DownloadedImage { ImageBytes = imageBytes, ContentType = contentType };
        }

        public async Task SaveThumbnail(string key, string contentType, MemoryStream outStream)
        {
            var putObjectRequest = new PutObjectRequest
            {
                Key = $"thumbnails/{key}",
                BucketName = _bucketName,
                ContentType = contentType,
                InputStream = outStream
            };
            await _amazonS3.PutObjectAsync(putObjectRequest);
        }
    }
}
