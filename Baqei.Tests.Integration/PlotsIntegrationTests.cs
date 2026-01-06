using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Baqei.Tests.Integration;

public class PlotsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PlotsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPlots_Unauthorized_WhenNoToken()
    {
        var response = await _client.GetAsync("/api/plots");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
