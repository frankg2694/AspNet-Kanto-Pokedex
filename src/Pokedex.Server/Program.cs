using Microsoft.Extensions.Caching.Hybrid;
using StackExchange.Redis;
using Pokedex.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register PokemonService with its own HttpClient
builder.Services.AddHttpClient<PokemonService>(client =>
{
    client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
});

// Get the Redis connection string
string? redisConnection = builder.Configuration.GetConnectionString("Redis");

// No connection string means no Redis, memory cache only
if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        // If Redis is down, fail fast and get data from PokeAPI
        var redisOptions = ConfigurationOptions.Parse(redisConnection);
        redisOptions.ConnectTimeout = 1000;
        redisOptions.AbortOnConnectFail = false;
        redisOptions.BacklogPolicy = BacklogPolicy.FailFast;
        options.ConfigurationOptions = redisOptions;
    });
}

// Register HybridCache, items expire after 7 days
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromDays(7)
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Load index.html by default
app.UseDefaultFiles();

// Make browsers check for a new index.html every time
var staticFileOptions = new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        if (context.File.Name == "index.html")
        {
            context.Context.Response.Headers.CacheControl = "no-cache";
        }
    }
};

// Serve the files in wwwroot
app.UseStaticFiles(staticFileOptions);

app.MapControllers();

// Any other URL goes to index.html
app.MapFallbackToFile("index.html", staticFileOptions);

app.Run();