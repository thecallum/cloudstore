using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Infrastructure;
using DocumentService.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.E2ETests
{
    public class GetStorageUsageE2ETests : BaseIntegrationTest
    {
        public GetStorageUsageE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
        }

        [Fact]
        public async Task WhenEntityDoesntExist_ReturnsZero()
        {
            // Arrange

            // Act
            var response = await GetStorageUsageRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetStorageUsageResponse>(response);
            responseContent.StorageUsage.Should().Be(0);
        }

        [Fact]
        public async Task WhenEntityExists_ReturnsCorrectValue()
        {
            // Arrange
            var entity = new DocumentStorageDb { UserId = _user.Id, StorageUsage = _fixture.Create<long>() };
            await SetupTestData(entity);

            // Act
            var response = await GetStorageUsageRequest();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetStorageUsageResponse>(response);
            responseContent.StorageUsage.Should().Be(entity.StorageUsage);
        }

        private async Task<HttpResponseMessage> GetStorageUsageRequest()
        {
            // setup request
            var uri = new Uri($"/document-service/api/storage/usage/", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            message.Method = HttpMethod.Get;
            message.Headers.Add(TokenService.Constants.AuthToken, _token);

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
    }
}
