using Microsoft.AspNetCore.Mvc;
using Pokedex.Server.Models;
using Pokedex.Server.Services;

namespace Pokedex.Server.Controllers;

[ApiController]
[Route("api/pokemon")]
public class PokemonController : ControllerBase
{
    public const string PokeApiDownMessage = "PokeAPI is not available right now, please try again later";
    private readonly PokemonService pokemonService;
    private readonly ILogger<PokemonController> logger; //used to write errors to the console and Output window

    public PokemonController(PokemonService pokemonService, ILogger<PokemonController> logger)
    {
        this.pokemonService = pokemonService;
        this.logger = logger;
    }

    // GET /api/pokemon
    // Load the Dex Number and Name of all 151 Pokemon, used to fill the dropdowns
    [HttpGet]
    public async Task<ActionResult<List<PokemonSummary>>> GetAll()
    {
        try
        {
            // Get the list from PokemonService
            List<PokemonSummary> summaries = await pokemonService.GetAllAsync();
            return Ok(summaries);
        }
        catch (HttpRequestException ex)
        {

            logger.LogError(ex, "Could not get the Pokemon list from PokeAPI");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, PokeApiDownMessage);
        }
    }

    // GET /api/pokemon/25
    // Load one Pokemon by its Dex Number
    [HttpGet("{dexNum:int}")]
    public async Task<ActionResult<Pokemon>> GetByDexNum(int dexNum)
    {
        if (dexNum < 1 || dexNum > PokemonService.KantoCount)
        {
            return BadRequest("Dex number must be between 1 and 151");
        }

        try
        {
            // Get the Pokemon from PokemonService
            Pokemon? pokemon = await pokemonService.GetByDexNumAsync(dexNum);

            // Error check in case PokeAPI sent nothing back
            if (pokemon == null)
            {
                return NotFound();
            }

            return Ok(pokemon);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get Pokemon #{DexNum} from PokeAPI", dexNum);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, PokeApiDownMessage);
        }
    }

    // GET /api/pokemon/random
    // Load a random Pokemon from the original 151
    [HttpGet("random")]
    public async Task<ActionResult<Pokemon>> GetRandom()
    {
        // Pick a random Dex Number, Next() leaves out the top number so 151 needs the + 1
        int dexNum = Random.Shared.Next(1, PokemonService.KantoCount + 1);

        try
        {
            // Get the Pokemon from PokemonService
            Pokemon? pokemon = await pokemonService.GetByDexNumAsync(dexNum);

            // Error check in case PokeAPI sent nothing back
            if (pokemon == null)
            {
                return NotFound();
            }

            return Ok(pokemon);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get Pokemon #{DexNum} from PokeAPI", dexNum);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, PokeApiDownMessage);
        }
    }
}