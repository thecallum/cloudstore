using DocumentService.Boundary.Request;
using DocumentService.Tests.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using FluentAssertions;
using System.Net;
using DocumentService.Domain;
using DocumentService.Infrastructure;

namespace DocumentService.Tests.E2ETests
{
    public class ValidateUploadedDocumentE2ETests : BaseIntegrationTest
    {
        private readonly string _validFilePath;
        private readonly string _tooLargeFilePath;

        private readonly S3TestHelper _s3TestHelper;

        public ValidateUploadedDocumentE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;
            _tooLargeFilePath = testFixture.TooLargeFilePath;

            _s3TestHelper = new S3TestHelper(_s3Client);
        }

        [Fact]
        public async Task WhenDocumentDoesntExist_ReturnsNull()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");

            var documentId = Guid.NewGuid();
            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                            .With(x => x.DirectoryId, Guid.NewGuid())
                            .Create();

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDocumentExists_ReturnsDocument()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");
            var documentId = Guid.NewGuid();
            var key = $"{userId}/{documentId}";

            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                            .With(x => x.DirectoryId, Guid.NewGuid())
                            .Create();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            response.Headers.Location.ToString().Should().Contain(documentId.ToString());

            var responseContent = await DecodeResponse<Document>(response);

            responseContent.Should().NotBeNull();
            responseContent.Id.Should().Be(documentId);
            responseContent.UserId.Should().Be(userId);
            responseContent.DirectoryId.Should().Be((Guid)request.DirectoryId);
            responseContent.FileSize.Should().Be(200);
            responseContent.Name.Should().Be(request.FileName);
            responseContent.S3Location.Should().Be($"{userId}/{documentId}");
        }

        [Fact]
        public async Task WhenDocumentExists_MovesDocumentToStoreDirectory()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");
            var documentId = Guid.NewGuid();
            var key = $"{userId}/{documentId}";

            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            await _s3TestHelper.VerifyDocumentUploadedToS3($"store/{key}");
            await _s3TestHelper.VerifyDocumentDeletedFromS3($"upload/{key}");
        }

        [Fact]
        public async Task WhenCalled_InsertsDocumentIntoDatabase()
        {
            // Arrange
            var userId = Guid.Parse("851944df-ac6a-43f1-9aac-f146f19078ed");
            var documentId = Guid.NewGuid();
            var key = $"{userId}/{documentId}";

            var request = _fixture.Build<ValidateUploadedDocumentRequest>()
                .With(x => x.DirectoryId, Guid.NewGuid())
                .Create();

            await _s3TestHelper.UploadDocumentToS3($"upload/{key}", _validFilePath);

            // Act
            var response = await ValidateUploadedDocumentRequest(documentId, request);

            // Assert
            var databaseResponse = await _context.LoadAsync<DocumentDb>(userId, documentId).ConfigureAwait(false);

            databaseResponse.Should().NotBeNull();
            databaseResponse.DocumentId.Should().Be(documentId);
            databaseResponse.UserId.Should().Be(userId);
            databaseResponse.DirectoryId.Should().Be((Guid)request.DirectoryId);
            databaseResponse.FileSize.Should().Be(200);
            databaseResponse.Name.Should().Be(request.FileName);
            databaseResponse.S3Location.Should().Be($"{userId}/{documentId}");
        }

        private async Task<HttpResponseMessage> ValidateUploadedDocumentRequest(Guid documentId, ValidateUploadedDocumentRequest request)
        {
            var uri = new Uri($"document-service/api/document/validate/{documentId}", UriKind.Relative);

            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, data).ConfigureAwait(false);

            return response;
        }
    }
}
