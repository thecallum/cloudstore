using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture;
using DocumentService.Domain;
using DocumentService.Gateways;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.Gateways
{
    public class SnsGatewayTests
    {
        private readonly ISnsGateway _snsGateway;
        private readonly Mock<IAmazonSimpleNotificationService> _mockAwsSns;

        private readonly Fixture _fixture = new Fixture();

        public SnsGatewayTests()
        {
            _mockAwsSns = new Mock<IAmazonSimpleNotificationService>();

            _snsGateway = new SnsGateway(_mockAwsSns.Object);
        }

        [Fact]
        public async Task PublishDocumentUploadedEvent_WhenCalled_PublishesEvent()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var documentId = Guid.NewGuid();

            // Act
            await _snsGateway.PublishDocumentUploadedEvent(user, documentId);

            // Assert
            _mockAwsSns
                .Verify(x => x.PublishAsync(It.IsAny<PublishRequest>(), default), Times.Once);
        }

        [Fact]
        public async Task PublishDocumentDeletedEvent_WhenCalled_PublishesEvent()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var documentId = Guid.NewGuid();

            // Act
            await _snsGateway.PublishDocumentDeletedEvent(user, documentId);

            // Assert
            _mockAwsSns
                .Verify(x => x.PublishAsync(It.IsAny<PublishRequest>(), default), Times.Once);
        }

        [Fact]
        public async Task PublishDeleteDirectoryEvent_WhenCalled_PublishesEvent()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var directoryId = Guid.NewGuid();

            // Act
            await _snsGateway.PublishDeleteDirectoryEvent(user, directoryId);

            // Assert
            _mockAwsSns
                .Verify(x => x.PublishAsync(It.IsAny<PublishRequest>(), default), Times.Once);
        }
    }
}
