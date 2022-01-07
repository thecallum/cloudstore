using AutoFixture;
using AWSServerless1;
using AWSServerless1.Formatters;
using AWSServerless1.Gateways;
using DocumentServiceListener.Boundary;
using DocumentServiceListener.Gateways;
using DocumentServiceListener.Gateways.Exceptions;
using DocumentServiceListener.Gateways.Interfaces;
using DocumentServiceListener.Gateways.Models;
using FluentAssertions;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.UseCase
{
    public class DocumentUploadedUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;
        private readonly Mock<IImageFormatter> _mockImageFormatter;
        private readonly Mock<IImageLoader> _mockImageLoader;

        private readonly DocumentUploadedUseCase _classUnderTest;

        public DocumentUploadedUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();
            _mockImageFormatter = new Mock<IImageFormatter>();
            _mockImageLoader = new Mock<IImageLoader>();

            _classUnderTest = new DocumentUploadedUseCase(
                _mockS3Gateway.Object,
                _mockDocumentGateway.Object,
                _mockImageFormatter.Object,
                _mockImageLoader.Object);
        }

        [Fact]
        public async Task WhenMissingBodyParameters_ThrowsArgumentNullException()
        {
            // Arrange
            var entity = _fixture.Create<CloudStoreSnsEvent>();

            // Act
            Func<Task> func = async () => await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task WhenDocumentDoesntExist_ReturnsNull()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DocumentId", documentId.ToString());

            _mockS3Gateway
                .Setup(x => x.GetObjectMetadata(It.IsAny<string>()))
                .ReturnsAsync((ObjectMetadata) null);

            // Act
            await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            _mockImageFormatter
                .Verify(x => x.EditImage(It.IsAny<Image>(), It.IsAny<MemoryStream>()), Times.Never);
        }

        [Fact]
        public async Task WhenInvalidFileFormat_SwallowsException()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DocumentId", documentId.ToString());

            var downloadedImageResponse = _fixture.Create<DownloadedImage>();

            _mockS3Gateway
                .Setup(x => x.DownloadImage(It.IsAny<string>()))
                .ReturnsAsync(downloadedImageResponse);

            _mockImageLoader
                .Setup(x => x.LoadImage(It.IsAny<Byte[]>()))
                .Throws(new UnknownImageFormatException(""));


            // Act
            Func<Task> func = async () => await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            await func.Should().NotThrowAsync<Exception>();

            _mockImageFormatter
                .Verify(x => x.EditImage(It.IsAny<Image>(), It.IsAny<MemoryStream>()), Times.Never);
        }

        [Fact]
        public async Task WhenDocumentTooLarge_SwallowsException()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DocumentId", documentId.ToString());

            var objectMetadataResponse = new ObjectMetadata { ContentLength = 204857600 };

            _mockS3Gateway
                .Setup(x => x.GetObjectMetadata(It.IsAny<string>()))
                .ReturnsAsync(objectMetadataResponse);

            // Act
            Func<Task> func = async () => await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            await func.Should().NotThrowAsync<Exception>();

            _mockImageFormatter
                .Verify(x => x.EditImage(It.IsAny<Image>(), It.IsAny<MemoryStream>()), Times.Never);
        }

        [Fact]
        public async Task WhenDocumentNotFoundInDb_ThrowsException()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DocumentId", documentId.ToString());

            var objectMetadataResponse = new ObjectMetadata { ContentLength = 100 };

            _mockS3Gateway
            .Setup(x => x.GetObjectMetadata(It.IsAny<string>()))
                .ReturnsAsync(objectMetadataResponse);


            var downloadedImageResponse = _fixture.Create<DownloadedImage>();

            _mockS3Gateway
                .Setup(x => x.DownloadImage(It.IsAny<string>()))
                .ReturnsAsync(downloadedImageResponse);

            var imageLoaderResponse = _fixture.Create<Image<Rgba32>>();

            _mockImageLoader
                .Setup(x => x.LoadImage(It.IsAny<Byte[]>()))
                .Returns(imageLoaderResponse);


            // throw document not found exception
            _mockDocumentGateway
                .Setup(x => x.UpdateThumbnail(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(new DocumentDbNotFoundException(documentId, Guid.NewGuid()));

            // Act
            Func<Task> func = async () => await _classUnderTest.ProcessMessageAsync(entity);

            // Assert
            await func.Should().ThrowAsync<DocumentDbNotFoundException>();
        }

        [Fact]
        public async Task WhenCalled_UpdatesImageThumbnail()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var entity = _fixture.Create<CloudStoreSnsEvent>();
            entity.Body.Add("DocumentId", documentId.ToString());

            var objectMetadataResponse = new ObjectMetadata { ContentLength = 100 };

            _mockS3Gateway
            .Setup(x => x.GetObjectMetadata(It.IsAny<string>()))
                .ReturnsAsync(objectMetadataResponse);


            var downloadedImageResponse = _fixture.Create<DownloadedImage>();

            _mockS3Gateway
                .Setup(x => x.DownloadImage(It.IsAny<string>()))
                .ReturnsAsync(downloadedImageResponse);

            var imageLoaderResponse = _fixture.Create<Image<Rgba32>>();

            _mockImageLoader
                .Setup(x => x.LoadImage(It.IsAny<Byte[]>()))
                .Returns(imageLoaderResponse);


            // Act
            await _classUnderTest.ProcessMessageAsync(entity);

            // Assert

            _mockImageFormatter
                .Verify(x => x.EditImage(It.IsAny<Image>(), It.IsAny<MemoryStream>()), Times.Once);

            _mockS3Gateway
                .Verify(x => x.SaveThumbnail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MemoryStream>()), Times.Once);

            _mockDocumentGateway
                .Verify(x => x.UpdateThumbnail(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

    }
}
