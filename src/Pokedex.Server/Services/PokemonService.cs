using Microsoft.Extensions.Caching.Hybrid;
using Pokedex.Server.Models;

namespace Pokedex.Server.Services;

// Service used to get the Pokemon data from PokeAPI (https://pokeapi.co). Results are saved in HybridCache (memory + Redis), so PokeAPI only gets called the first time
public class PokemonService
{
    public const int KantoCount = 151;
    private const string ArtworkBaseUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/";
    private readonly HttpClient httpClient;
    private readonly HybridCache cache;

    // ASP.NET Core passes in the HttpClient and the HybridCache registered in Program.cs
    public PokemonService(HttpClient httpClient, HybridCache cache)
    {
        this.httpClient = httpClient;
        this.cache = cache;
    }

    // Get the Dex Number and Name of all 151 Kanto Pokemon (used to fill the dropdowns)
    public async Task<List<PokemonSummary>> GetAllAsync()
    {
        // Look for "pokemon:all" in the cache, if it's not there HybridCache calls GetAllFromPokeApiAsync and saves the result
        // Note: No bracket after GetAllFromPokeApiAsync allows HybridCache to decide if GetAllFromPokeApiAsync needs to be called. If there was a bracket after it, it will always be called regardless if cached
        List<PokemonSummary> summaries = await cache.GetOrCreateAsync("pokemon:all", GetAllFromPokeApiAsync);

        return summaries;
    }

    // Get one Pokemon by its Dex Number (used by the dropdowns and by Random)
    public async Task<Pokemon?> GetByDexNumAsync(int dexNum)
    {
        // Look for ex: "pokemon:25" in the cache, if it's not there HybridCache calls GetByDexNumFromPokeApiAsync with dexNum and saves the result
        Pokemon? pokemon = await cache.GetOrCreateAsync("pokemon:" + dexNum, dexNum, GetByDexNumFromPokeApiAsync);

        return pokemon;
    }

    // Call PokeAPI for the list of the first 151 Pokemon
    private async ValueTask<List<PokemonSummary>> GetAllFromPokeApiAsync(CancellationToken cancel)
    {
        List<PokemonSummary> summaries = new List<PokemonSummary>();

        PokeApiPokemonList? apiList = await httpClient.GetFromJsonAsync<PokeApiPokemonList>("pokemon?limit=" + KantoCount, cancel);

        // Error check in case PokeAPI sent nothing back
        if (apiList == null)
        {
            return summaries;
        }

        // PokeAPI returns the list in Dex order
        for (int i = 0; i < apiList.Results.Count; i++)
        {
            PokemonSummary summary = new PokemonSummary();
            summary.DexNum = i + 1;
            summary.Name = FormatName(apiList.Results[i].Name);
            summaries.Add(summary);
        }

        return summaries;
    }

    // Call PokeAPI for one Pokemon and convert it to the app's Pokemon class
    private async ValueTask<Pokemon?> GetByDexNumFromPokeApiAsync(int dexNum, CancellationToken cancel)
    {
        PokeApiPokemon? apiPokemon = await httpClient.GetFromJsonAsync<PokeApiPokemon>("pokemon/" + dexNum, cancel);

        // Error check in case PokeAPI sent nothing back
        if (apiPokemon == null)
        {
            return null;
        }

        Pokemon pokemon = new Pokemon();
        pokemon.DexNum = apiPokemon.Id;
        pokemon.Name = FormatName(apiPokemon.Name);
        pokemon.Picture = ArtworkBaseUrl + apiPokemon.Id + ".png";

        foreach (PokeApiTypeSlot typeSlot in apiPokemon.Types)
        {
            if (typeSlot.Slot == 1)
            {
                pokemon.TypeOne = Capitalize(typeSlot.Type.Name);
            }
            else if (typeSlot.Slot == 2)
            {
                pokemon.TypeTwo = Capitalize(typeSlot.Type.Name);
            }
        }

        return pokemon;
    }

    // Format the Pokemon names correctly
    private static string FormatName(string apiName)
    {
        switch (apiName)
        {
            case "nidoran-f":
                return "Nidoran♀";
            case "nidoran-m":
                return "Nidoran♂";
            case "farfetchd":
                return "Farfetch'd";
            case "mr-mime":
                return "Mr. Mime";
            default:
                return Capitalize(apiName);
        }
    }

    // Capitalize the first letter because PokeAPI names are lowercase
    private static string Capitalize(string text)
    {
        if (text == "")
        {
            return text;
        }

        return char.ToUpper(text[0]) + text.Substring(1);
    }
}