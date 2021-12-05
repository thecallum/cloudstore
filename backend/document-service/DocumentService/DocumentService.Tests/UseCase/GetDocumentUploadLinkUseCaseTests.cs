using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Gateways;
using DocumentService.Gateways.Interfaces;
using DocumentService.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.UseCase
{
    public class GetDocumentUploadLinkUseCaseTests
    {
        private readonly GetDocumentUploadLinkUseCase _useCase;
        private readonly Mock<IS3Gateway> _mockS3Gateway;

        private readonly Fixture _fixture = new Fixture();

        public GetDocumentUploadLinkUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();

            _useCase = new GetDocumentUploadLinkUseCase(_mockS3Gateway.Object);
        }

        [Fact]
        public void WhenExistingDocumentIdNull_CreatedNewDocumentId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockUrl = _fixture.Create<string>();

            var query = new GetDocumentUploadLinkQuery { };

            _mockS3Gateway
                .Setup(x => x.GetDocumentUploadPresignedUrl(It.IsAny<string>()))
                .Returns(mockUrl);

            // Act
            var response = _useCase.Execute(userId, query);

            // Assert
            response.UploadUrl.Should().Be(mockUrl);
        }

        [Fact]
        public void WhenExistingDocumentIdNotNull_UsesExistingDocumentId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockUrl = _fixture.Create<string>();

            var query = new GetDocumentUploadLinkQuery { ExistingDocumentId = Guid.NewGuid() };

            _mockS3Gateway
                .Setup(x => x.GetDocumentUploadPresignedUrl(It.IsAny<string>()))
                .Returns(mockUrl);

            // Act
            var response = _useCase.Execute(userId, query);

            // Assert
            response.UploadUrl.Should().Be(mockUrl);
            response.DocumentId.Should().Be((Guid)query.ExistingDocumentId);
        }
    }
}
