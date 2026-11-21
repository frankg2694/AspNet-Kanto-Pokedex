namespace Pokedex.Server.Models;

// A Pokemon, the way it gets returned to the React Client
public class Pokemon
{
    // National Dex Number (1-151)
    public int DexNum { get; set; }

    public string Name { get; set; } = "";

    // First type, every Pokemon has at least one
    public string TypeOne { get; set; } = "";

    // Second type, or null if the Pokemon only has one
    public string? TypeTwo { get; set; }

    // URL of the official artwork
    public string Picture { get; set; } = "";
}