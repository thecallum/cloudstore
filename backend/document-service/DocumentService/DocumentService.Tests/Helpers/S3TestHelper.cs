using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Tests.Helpers
{
    public class S3TestHelper
    {
        private const string BucketName = "uploadfromcs";
        private readonly IAmazonS3 _s3Client;

        public S3TestHelper(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task DeleteDocumentFromS3(string key)
        {

            var request = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
        }

        public async Task VerifyDocumentUploadedToS3(string key)
        {
            // test file exists
            var request = new GetObjectMetadataRequest
            {
                BucketName = BucketName,
                Key = key
            };

            try
            {
                await _s3Client.GetObjectMetadataAsync(request);
            }
            catch (Exception)
            {
                throw new Exception("Document Metadata could lot be loaded from s3");
            }
        }

        public async Task VerifyDocumentDeletedFromS3(string key)
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = BucketName,
                Key = key
            };

            try
            {
                await _s3Client.GetObjectMetadataAsync(request);

                throw new Exception("Document still found in s3");
            }
            catch (Exception)
            {
                // should throw error
            }
        }

        public async Task UploadDocumentToS3(string key, string filePath)
        {
            using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var s3Request = new PutObjectRequest()
                {
                    InputStream = inputStream,
                    BucketName = BucketName,
                    Key = key
                };

                await _s3Client.PutObjectAsync(s3Request);
            }
        }
    }
}
