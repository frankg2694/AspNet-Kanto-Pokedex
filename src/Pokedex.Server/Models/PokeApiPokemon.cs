namespace Pokedex.Server.Models;

// Only the parts of the PokeAPI /pokemon/{id} response this app uses
// Full response: https://pokeapi.co/docs/v2#pokemon
public class PokeApiPokemon
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<PokeApiTypeSlot> Types { get; set; } = new List<PokeApiTypeSlot>();
}

// One entry in the "types" list, slot 1 is the first type and slot 2 is the second type
public class PokeApiTypeSlot
{
    public int Slot { get; set; }

    public PokeApiType Type { get; set; } = new PokeApiType();
}

// The type itself, for example "grass"
public class PokeApiType
{
    public string Name { get; set; } = "";
}