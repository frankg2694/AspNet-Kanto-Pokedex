namespace Pokedex.Server.Models;

// Only the parts of the PokeAPI /pokemon?limit=151 response this app uses
public class PokeApiPokemonList
{
    public List<PokeApiListItem> Results { get; set; } = new List<PokeApiListItem>();
}

// One Pokemon in the list, for example "pikachu"
public class PokeApiListItem
{
    public string Name { get; set; } = "";
}