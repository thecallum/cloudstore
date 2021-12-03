using AutoFixture;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using DocumentServiceListener.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.Gateways
{
    public class S3GatewayTests : BaseIntegrationTest
    {
        private readonly IS3Gateway _gateway;

        private readonly string _validFilePath;

        private readonly S3TestHelper _s3TestHelper;


        public S3GatewayTests(DatabaseFixture testFixture)
            : base(testFixture)
        {
            _gateway = new S3Gateway(_s3Client);

            _validFilePath = testFixture.ValidFilePath;

            _s3TestHelper = new S3TestHelper(_s3Client);

        }

        [Fact]
        public async Task DeleteDocuments_WhenCalled_DeletesObjectsFromS3()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documents = _fixture.Build<DocumentDb>()
                .With(x => x.UserId, userId)
                .CreateMany(3).ToList();

            foreach (var document in documents)
            {
                var key = $"store/{document.UserId}/{document.DocumentId}";

                await _s3TestHelper.UploadDocumentToS3(key, _validFilePath);
            }

            // Act
            await _gateway.DeleteDocuments(documents, userId);

            // Assert
            foreach(var document in documents)
            {
                var key = $"store/{document.UserId}/{document.DocumentId}";

                await _s3TestHelper.VerifyDocumentDeletedFromS3(key);
            }
        }
    }
}
