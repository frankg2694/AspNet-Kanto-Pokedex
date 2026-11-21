using Pokedex.Server.Models;
using Pokedex.Server.Services;

namespace Pokedex.Server.Tests;

// Tests for PokemonService, using FakePokeApiHandler instead of the real PokeAPI
public class PokemonServiceTests
{
    // Check Pikachu comes back with the right Dex Number, Name, Type and Picture
    [Fact]
    public async Task GetByDexNumAsync_Pikachu_ReturnsCapitalizedNameTypeAndPicture()
    {
        string pikachuJson = """
            {
              "id": 25,
              "name": "pikachu",
              "types": [
                { "slot": 1, "type": { "name": "electric" } }
              ]
            }
            """;

        FakePokeApiHandler handler = new FakePokeApiHandler(pikachuJson);
        PokemonService service = CreateService(handler);

        Pokemon? pokemon = await service.GetByDexNumAsync(25);

        Assert.NotNull(pokemon);
        Assert.Equal("https://pokeapi.co/api/v2/pokemon/25", handler.LastRequestUrl);
        Assert.Equal(25, pokemon.DexNum);
        Assert.Equal("Pikachu", pokemon.Name);
        Assert.Equal("Electric", pokemon.TypeOne);
        Assert.EndsWith("/official-artwork/25.png", pokemon.Picture);
    }

    // Check a Pokemon has only one Type
    [Fact]
    public async Task GetByDexNumAsync_SingleTypePokemon_TypeTwoIsNull()
    {
        string charmanderJson = """
            {
              "id": 4,
              "name": "charmander",
              "types": [
                { "slot": 1, "type": { "name": "fire" } }
              ]
            }
            """;

        FakePokeApiHandler handler = new FakePokeApiHandler(charmanderJson);
        PokemonService service = CreateService(handler);

        Pokemon? pokemon = await service.GetByDexNumAsync(4);

        Assert.NotNull(pokemon);
        Assert.Equal("Fire", pokemon.TypeOne);
        Assert.Null(pokemon.TypeTwo);
    }

    // Check the list of Pokemon comes back in Dex order with capitalized Names
    [Fact]
    public async Task GetAllAsync_ReturnsDexNumbersInOrderWithCapitalizedNames()
    {
        string listJson = """
            {
              "count": 1302,
              "results": [
                { "name": "bulbasaur", "url": "https://pokeapi.co/api/v2/pokemon/1/" },
                { "name": "ivysaur", "url": "https://pokeapi.co/api/v2/pokemon/2/" },
                { "name": "venusaur", "url": "https://pokeapi.co/api/v2/pokemon/3/" }
              ]
            }
            """;

        FakePokeApiHandler handler = new FakePokeApiHandler(listJson);
        PokemonService service = CreateService(handler);

        List<PokemonSummary> summaries = await service.GetAllAsync();

        Assert.Equal("https://pokeapi.co/api/v2/pokemon?limit=151", handler.LastRequestUrl);
        Assert.Equal(3, summaries.Count);

        Assert.Equal(1, summaries[0].DexNum);
        Assert.Equal("Bulbasaur", summaries[0].Name);

        Assert.Equal(2, summaries[1].DexNum);
        Assert.Equal("Ivysaur", summaries[1].Name);

        Assert.Equal(3, summaries[2].DexNum);
        Assert.Equal("Venusaur", summaries[2].Name);
    }

    // Check the second call gets Pikachu from the cache instead of calling PokeAPI again
    [Fact]
    public async Task GetByDexNumAsync_SecondCall_UsesCacheInsteadOfPokeApi()
    {
        string pikachuJson = """
            {
              "id": 25,
              "name": "pikachu",
              "types": [
                { "slot": 1, "type": { "name": "electric" } }
              ]
            }
            """;

        FakePokeApiHandler handler = new FakePokeApiHandler(pikachuJson);
        PokemonService service = CreateService(handler);

        Pokemon? firstPokemon = await service.GetByDexNumAsync(25);
        Pokemon? secondPokemon = await service.GetByDexNumAsync(25);

        Assert.Equal(1, handler.RequestCount);
        Assert.NotNull(firstPokemon);
        Assert.NotNull(secondPokemon);
        Assert.Equal("Pikachu", secondPokemon.Name);
    }

    // Create a PokemonService that uses the fake handler instead of the real PokeAPI
    private static PokemonService CreateService(FakePokeApiHandler handler)
    {
        HttpClient httpClient = new HttpClient(handler);
        httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        return new PokemonService(httpClient, TestCache.Create());
    }

    // Check the Pokemon names are formatted correctly
    [Theory]
    [InlineData("pikachu", "Pikachu")]
    [InlineData("nidoran-f", "Nidoran♀")]
    [InlineData("nidoran-m", "Nidoran♂")]
    [InlineData("farfetchd", "Farfetch'd")]
    [InlineData("mr-mime", "Mr. Mime")]
    public async Task GetByDexNumAsync_FormatsNameForDisplay(string apiName, string expectedName)
    {
        string json = "{ \"id\": 1, \"name\": \"" + apiName + "\", \"types\": [] }";

        FakePokeApiHandler handler = new FakePokeApiHandler(json);
        PokemonService service = CreateService(handler);

        Pokemon? pokemon = await service.GetByDexNumAsync(1);

        Assert.NotNull(pokemon);
        Assert.Equal(expectedName, pokemon.Name);
    }
}