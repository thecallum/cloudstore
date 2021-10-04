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
        private readonly int _maxFileSize;
        public S3Gateway(IAmazonS3 amazonS3, int maxFileSize = 1073741824)
        {
            _maxFileSize = maxFileSize;
            _bucketName = "uploadfromcs";
            _amazonS3 = amazonS3;
        }
        public async Task<DocumentUploadResponse> UploadDocument(UploadDocumentRequest request, Guid documentId, Guid userId)
        {
            var s3Location = $"{userId}/{documentId}";
            long fileSize;

            try
            {
                using (FileStream inputStream = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                {
                    fileSize = inputStream.Length;

                    if (fileSize > _maxFileSize) throw new FileTooLargeException();
        
                    var s3Request = new PutObjectRequest()
                    {
                        InputStream = inputStream,
                        BucketName = _bucketName,
                        Key = s3Location
                    };

                    await _amazonS3.PutObjectAsync(s3Request);
                }

                return new DocumentUploadResponse
                {
                    DocumentName = Path.GetFileName(request.FilePath),
                    S3Location = s3Location,
                    FileSize = fileSize
                };
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is System.IO.DirectoryNotFoundException)
                {
                    throw new InvalidFilePathException();
                }

                throw ex;
            }
        }

        public string GetDocumentPresignedUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Expires = DateTime.UtcNow.AddMinutes(20),
                Key = key
            };

            return _amazonS3.GetPreSignedURL(request);
        }
    }
}
