using AutoFixture;
using DocumentService.Boundary.Request;
using DocumentService.Boundary.Response;
using DocumentService.Domain;
using DocumentService.Factories;
using DocumentService.Infrastructure;
using DocumentService.Tests.Helpers;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Tests.E2ETests
{
    public class GetAllDocumentsE2ETests : BaseIntegrationTest
    {
        public GetAllDocumentsE2ETests(DatabaseFixture<Startup> testFixture)
            : base(testFixture)
        {
        }

        [Fact]
        public async Task WhenNoDocumentsExist_ReturnsNoDocuments()
        {
           // Arrange
           var query = new GetAllDocumentsQuery { DirectoryId = null };

           // Act
           var response = await GetAllDocumentsRequest(query);

           // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await DecodeResponse<GetAllDocumentsResponse>(response);

            responseContent.Documents.Should().HaveCount(0);
        }

        [Fact]
        public async Task WhenManyDocumentsExist_ReturnsManyDocuments()
        {
           // Arrange
           var query = new GetAllDocumentsQuery { DirectoryId = null };

            var numberOfDocuments = _random.Next(2, 5);
            var mockDocuments = _fixture
                .Build<DocumentDomain>()
                .With(x => x.UserId, _user.Id)
                .With(x => x.DirectoryId, (Guid?) null)
                .CreateMany(numberOfDocuments)
                .Select(x => x.ToDatabase());

            await _dbFixture.SetupTestData(mockDocuments);

           // Act
           var response = await GetAllDocumentsRequest(query);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            //test contents of response
            var responseContent = await DecodeResponse<GetAllDocumentsResponse>(response);

            responseContent.Documents.Should().HaveCount(numberOfDocuments);
        }

        private async Task<HttpResponseMessage> GetAllDocumentsRequest(GetAllDocumentsQuery query)
        {
            // setup request
            var uri = new Uri($"/document-service/api/document/", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            message.Method = HttpMethod.Get;
            message.Headers.Add(TokenService.Constants.AuthToken, _token);

           // message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

           // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }
      }
  }
