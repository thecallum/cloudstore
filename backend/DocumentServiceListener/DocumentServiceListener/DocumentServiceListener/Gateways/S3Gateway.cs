using Amazon.S3;
using Amazon.S3.Model;
using DocumentServiceListener.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentServiceListener.Gateways
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

        public async Task DeleteDocuments(List<DocumentDb> documents, Guid userId)
        {
            var request = new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = new List<KeyVersion>()
            };

            foreach(var document in documents)
            {
                var keyObject = new KeyVersion { Key = $"store/{document.UserId}/{document.DocumentId}" };

                request.Objects.Add(keyObject);
            }

            await _amazonS3.DeleteObjectsAsync(request);
        }
    }
}
