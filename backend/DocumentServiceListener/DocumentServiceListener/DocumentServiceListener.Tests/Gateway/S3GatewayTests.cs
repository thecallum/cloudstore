﻿using AutoFixture;
using AWSServerless1.Gateways;
using DocumentServiceListener.Tests.Helpers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentServiceListener.Tests.Gateway
{
    [Collection("Database collection")]
    public class S3GatewayTests : IDisposable
    {
        private readonly S3Gateway _gateway;
        private readonly Fixture _fixture = new Fixture();
        private readonly S3TestHelper _s3TestHelper;
        private readonly string _validFilePath;

        public S3GatewayTests(DatabaseFixture testFixture)
        {
            _validFilePath = testFixture.ValidFilePath;

            _gateway = new S3Gateway(testFixture.S3Client);
            _s3TestHelper = new S3TestHelper(testFixture.S3Client);
        }

        public void Dispose()
        {
           //
        }


        [Fact]
        public async Task GetObjectMetadata_WhenObjectNotFound_ReturnsNull()
        {
            // Arrange
            var key = _fixture.Create<string>();

            // Act
            var response = await _gateway.GetObjectMetadata(key);

            // Assert
            response.Should().BeNull();

        }

        [Fact]
        public async Task GetObjectMetadata_WhenObjectExists_ReturnsObjectMetadata()
        {
            // Arrange
            var key = _fixture.Create<string>();
            await _s3TestHelper.UploadDocumentToS3($"store/{key}", _validFilePath);

            // Act
            var response = await _gateway.GetObjectMetadata(key);

            // Assert
            response.Should().NotBeNull();
            response.ContentLength.Should().Be(200);
        }

        [Fact]
        public async Task DownloadImage_WhenCalled_DownloadsImageAsBytes()
        {
            // Arrange
            var key = _fixture.Create<string>();
            await _s3TestHelper.UploadDocumentToS3($"store/{key}", _validFilePath);

            // Act
            var response = await _gateway.DownloadImage(key);

            // Assert
            response.Should().NotBeNull();
            response.ImageBytes.Should().HaveCount(200);
            response.ContentType.Should().Be("text/plain");

        }

        [Fact]
        public async Task SaveThumbnail_WhenCalled_UploadsThumbnailToS3()
        {
            // Arrange
            var key = _fixture.Create<string>();
            var contentType = "image/jpeg";

            var memoryStream = new MemoryStream(File.ReadAllBytes(_validFilePath));

            // Act
            await _gateway.SaveThumbnail(key, contentType, memoryStream);

            // Assert
            await _s3TestHelper.VerifyDocumentUploadedToS3($"thumbnails/{key}");

        }


       
    }
}
