using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Pokedex.Server.Controllers;
using Pokedex.Server.Models;
using Pokedex.Server.Services;

namespace Pokedex.Server.Tests;

// Tests for PokemonController, using FakePokeApiHandler instead of the real PokeAPI
public class PokemonControllerTests
{
    // Check asking for Mew returns 200 OK with the Pokemon
    [Fact]
    public async Task GetByDexNum_Mew_ReturnsOkWithPokemon()
    {
        string mewJson = """
            {
              "id": 151,
              "name": "mew",
              "types": [
                { "slot": 1, "type": { "name": "psychic" } }
              ]
            }
            """;

        FakePokeApiHandler handler = new FakePokeApiHandler(mewJson);
        HttpClient httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        PokemonService service = new PokemonService(httpClient, TestCache.Create());
        // NullLogger ignores the log output
        PokemonController controller = new PokemonController(service, NullLogger<PokemonController>.Instance);

        ActionResult<Pokemon> result = await controller.GetByDexNum(151);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        Pokemon pokemon = Assert.IsType<Pokemon>(ok.Value);
        Assert.Equal(151, pokemon.DexNum);
        Assert.Equal("Mew", pokemon.Name);
        Assert.Equal("https://pokeapi.co/api/v2/pokemon/151", handler.LastRequestUrl);
    }

    // Check a Dex Number outside 1-151 returns 400 Bad Request without calling PokeAPI
    [Theory]
    [InlineData(0)]
    [InlineData(152)]
    public async Task GetByDexNum_OutsideKanto_ReturnsBadRequest(int dexNum)
    {
        FakePokeApiHandler handler = new FakePokeApiHandler("{}");
        HttpClient httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        PokemonService service = new PokemonService(httpClient, TestCache.Create());
        PokemonController controller = new PokemonController(service, NullLogger<PokemonController>.Instance);

        ActionResult<Pokemon> result = await controller.GetByDexNum(dexNum);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Dex number must be between 1 and 151", badRequest.Value);
        Assert.Null(handler.LastRequestUrl);
    }

    // Check the Server returns 503 when PokeAPI is down
    [Fact]
    public async Task GetByDexNum_PokeApiDown_Returns503()
    {
        FakePokeApiHandler handler = new FakePokeApiHandler("{}");
        handler.StatusCode = HttpStatusCode.InternalServerError;
        HttpClient httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        PokemonService service = new PokemonService(httpClient, TestCache.Create());
        PokemonController controller = new PokemonController(service, NullLogger<PokemonController>.Instance);

        ActionResult<Pokemon> result = await controller.GetByDexNum(25);

        ObjectResult error = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(503, error.StatusCode);
        Assert.Equal(PokemonController.PokeApiDownMessage, error.Value);
    }
}