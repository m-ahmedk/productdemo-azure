using FluentAssertions;
using System.Net;

namespace ProductDemo.IntegrationTests
{
    public class SmokeTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SmokeTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(); // <-- Bootstraps API
        }

        [Fact]
        public async Task Get_Swagger_Endpoint_Should_Work()
        {
            // Act
            var response = await _client.GetAsync("/swagger");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}