﻿using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Gateways;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Exceptions;
using DocumentService.UseCase;
using DocumentService.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;      

namespace DocumentService.Tests.UseCase
{
    public class GetDocumentLinkUseCaseTests
    {
        private readonly IGetDocumentLinkUseCase _getDocumentLinkUseCase;
        private readonly Mock<IS3Gateway> _mockS3Gateway;
        private readonly Mock<IDocumentGateway> _mockDocumentGateway;

        private readonly Fixture _fixture = new Fixture();

        public GetDocumentLinkUseCaseTests()
        {
            _mockS3Gateway = new Mock<IS3Gateway>();
            _mockDocumentGateway = new Mock<IDocumentGateway>();

            _getDocumentLinkUseCase = new GetDocumentLinkUseCase(_mockS3Gateway.Object, _mockDocumentGateway.Object);
        }

        [Fact]
        public async Task GetDocumentLink_WhenDocumentNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();

            _mockDocumentGateway
                .Setup(x => x.GetDocumentById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((DocumentDb) null);

            // Act
            Func<Task<string>> func = async () => await _getDocumentLinkUseCase.Execute(userId, documentId);

            // Assert
            await func.Should().ThrowAsync<DocumentNotFoundException>();
        }

        [Fact]
        public async Task GetDocumentLink_WhenDocumentFound_CallsGateway()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var documentId = Guid.NewGuid();

            var document = _fixture.Create<DocumentDb>();

            _mockDocumentGateway
                .Setup(x => x.GetDocumentById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(document);

            // Act
            var response = await _getDocumentLinkUseCase.Execute(userId, documentId);

            // Assert
            _mockS3Gateway.Verify(x => x.GetDocumentPresignedUrl(document.S3Location), Times.Once);
        }
    }
}
