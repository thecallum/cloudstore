using AutoFixture;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Infrastructure;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.Gateway
{
    [Collection("Database collection")]
    public class DocumentGatewayTests : IDisposable
    {
        private readonly DocumentGateway _gateway;
        private readonly Fixture _fixture = new Fixture();

        public DocumentGatewayTests()
        {
            _gateway = new DocumentGateway(InMemoryDb.Instance);

        }

        public void Dispose()
        {
            InMemoryDb.Teardown();
        }

        [Fact]
        public async Task UpdateThumbnail_WhenCalled_UpdatesImageThumbnail()
        {
            // Arrange
            var document = _fixture.Create<DocumentDb>();
            await SetupTestData(document);

            // Act
            await _gateway.UpdateThumbnail(document.UserId, document.Id);

            // Assert
            var expectedThumbnailUrl = $"https://uploadfromcs.s3.eu-west-1.amazonaws.com/thumbnails/{document.UserId}/{document.Id}";

            var databaseResponse = await LoadDocument(document.Id);
            databaseResponse.Thumbnail.Should().Be(expectedThumbnailUrl);
        }

        private async Task<DocumentDb> LoadDocument(Guid id)
        {
            return await InMemoryDb.Instance.Documents.FindAsync(id);
        }

        private async Task SetupTestData(DocumentDb document)
        {
            InMemoryDb.Instance.Documents.Add(document);

            await InMemoryDb.Instance.SaveChangesAsync();
        }
    }
}
