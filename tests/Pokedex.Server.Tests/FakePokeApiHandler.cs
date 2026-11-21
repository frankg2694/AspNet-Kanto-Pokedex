using System.Net;
using System.Text;

namespace Pokedex.Server.Tests;

// Fake PokeAPI so the tests never go on the internet, every request gets back the JSON the test passed in
public class FakePokeApiHandler : HttpMessageHandler
{
    private readonly string json;

    // 200 OK unless a test changes it to pretend PokeAPI is down
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    // Used by the tests to check what PokemonService asked for and how many times
    public string? LastRequestUrl { get; private set; }
    public int RequestCount { get; private set; }

    public FakePokeApiHandler(string json)
    {
        this.json = json;
    }

    // HttpClient calls this instead of sending a real request
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequestUrl = request.RequestUri?.ToString();
        RequestCount++;

        HttpResponseMessage response = new HttpResponseMessage(StatusCode);
        response.Content = new StringContent(json, Encoding.UTF8, "application/json");

        return Task.FromResult(response);
    }
}