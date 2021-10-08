using Amazon.S3;
using Amazon.S3.Model;
using DocumentService.Boundary.Request;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Gateways
{
    public class S3Gateway : IS3Gateway
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;
        public S3Gateway(IAmazonS3 amazonS3)
        {
            _bucketName = "uploadfromcs";
            _amazonS3 = amazonS3;
        }

        public async Task DeleteDocument(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = $"store/{key}"
            };

            await _amazonS3.DeleteObjectAsync(request);
        }

        private async Task DeleteUploadDocument(string key)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = $"upload/{key}"
            };


            await _amazonS3.DeleteObjectAsync(request);
        }

        public string GetDocumentUploadPresignedUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Expires = DateTime.UtcNow.AddMinutes(1),
                Verb = HttpVerb.PUT,
                Key = $"upload/{key}"

            };

            return _amazonS3.GetPreSignedURL(request);
        }

        public string GetDocumentDownloadPresignedUrl(string key, string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Expires = DateTime.UtcNow.AddMinutes(20),
                Verb = HttpVerb.GET,
                Key = $"store/{key}"
            };

            request.ResponseHeaderOverrides = new ResponseHeaderOverrides
            {
              ContentDisposition = $"attachment; filename = \"{fileName}\""
            };

            return _amazonS3.GetPreSignedURL(request);
        }

        public async Task<DocumentUploadResponse> ValidateUploadedDocument(string key)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = $"upload/{key}"
            };

            try
            {
                var response = await _amazonS3.GetObjectMetadataAsync(request);

                return new DocumentUploadResponse
                {
                    FileSize = response.ContentLength
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task MoveDocumentToStoreDirectory(string key)
        {
            await MoveDocument(key);
            await DeleteUploadDocument(key);
        }

        private async Task MoveDocument(string key)
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                DestinationBucket = _bucketName,
                SourceKey = $"upload/{key}",
                DestinationKey = $"store/{key}",            
                
            };

            await _amazonS3.CopyObjectAsync(request);
        }
    }
}
