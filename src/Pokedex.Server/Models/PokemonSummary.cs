namespace Pokedex.Server.Models;

// Short form of a Pokemon (AKA just the Dex Number and Name), used to fill the dropdowns
public class PokemonSummary
{
    public int DexNum { get; set; }

    public string Name { get; set; } = "";
}
