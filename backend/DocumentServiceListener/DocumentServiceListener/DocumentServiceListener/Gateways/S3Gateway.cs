using Amazon.S3;
using Amazon.S3.Model;
using DocumentServiceListener.Gateways.Models;
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

        public S3Gateway(IAmazonS3 amazonS3)
        {
            _bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
            _amazonS3 = amazonS3;
        }

        public async Task<ObjectMetadata> GetObjectMetadata(string key)
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

                return new ObjectMetadata { ContentLength = response.ContentLength };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DownloadedImage> DownloadImage(string key)
        { 
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"store/{key}"
            };

            byte[] imageBytes = null;
            var contentType = "";

            using (var objectResponse = await _amazonS3.GetObjectAsync(request))
            using (var ms = new MemoryStream())
            {
                contentType = objectResponse.Headers.ContentType;
                await objectResponse.ResponseStream.CopyToAsync(ms);
                imageBytes = ms.ToArray();
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

        public async Task DeleteThumbnail(string key)
        {
            Console.WriteLine($"Deleting thumbnail with key {key}");

            await DeleteDocument($"thumbnails/{key}");
        }

        private async Task DeleteDocument(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _amazonS3.DeleteObjectAsync(request);
        }

        public async Task DeleteDocuments(List<string> keys)
        {
            var request = new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = keys.Select(x => new KeyVersion { Key = x }).ToList()
            };

            await _amazonS3.DeleteObjectsAsync(request);
        }
    }
}
