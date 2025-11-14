using System.Net;
using System.Net.Http.Json;
using Flowmind;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Flowmind.Tests.Integration
{
    public class CheckinControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CheckinControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCheckinsV1_ShouldReturnPaginatedResponse()
        {
            var response = await _client.GetAsync("/api/users/1/checkins?page=1&pageSize=5");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadFromJsonAsync<object>();
            Assert.NotNull(json);
            var str = json!.ToString()!.ToLower();
            Assert.Contains("totalpages", str);
            Assert.Contains("items", str);
        }

        [Fact]
        public async Task GetCheckinsV2_ShouldReturnHateoasAndIndice()
        {
            var response = await _client.GetAsync("/api/v2/checkin");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadFromJsonAsync<object>();
            Assert.NotNull(json);
            var str = json!.ToString()!.ToLower();
            Assert.Contains("links", str);
            Assert.Contains("indiceequilibrio", str);
        }

        [Fact]
        public async Task PostCheckinV2_ShouldReturn201Created()
        {
            var payload = new { humor = "Calmo", energia = "Alta", sono = "Bom" };
            var response = await _client.PostAsJsonAsync("/api/v2/checkin", payload);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var location = response.Headers.Location?.ToString();
            Assert.NotNull(location);
            Assert.Contains("/api/v2/checkin/", location);
        }
    }
}