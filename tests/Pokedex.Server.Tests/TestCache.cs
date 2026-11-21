using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace Pokedex.Server.Tests;

// Builds a HybridCache for the tests, memory only so Redis doesn't need to be running
public static class TestCache
{
    public static HybridCache Create()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddHybridCache();

        ServiceProvider provider = services.BuildServiceProvider();
        return provider.GetRequiredService<HybridCache>();
    }
}