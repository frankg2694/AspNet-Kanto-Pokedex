using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Pokedex.Server.Tests;

// Tests for the /healthz endpoint, running the whole app in memory
public class HealthzTests
{
    // Check /healthz returns 200 OK with the text "OK"
    [Fact]
    public async Task Healthz_ReturnsOk()
    {
        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/healthz", TestContext.Current.CancellationToken);
        string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("OK", body);

        factory.Dispose();
    }
}
